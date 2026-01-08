using Controller;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using VContainer;

namespace Player {
    /// <summary>
    /// TrackingModuleから取得したコントローラー位置と
    /// キャラクターの基準位置の差分を使ってIKターゲットを制御
    /// </summary>
    public class HandIKController : SerializedMonoBehaviour {
        
        [Header("IK Targets")]
        [SerializeField]
        private Transform _leftHandIKTarget;
        
        [SerializeField]
        private Transform _rightHandIKTarget;
        
        [Header("Base References")]
        [SerializeField]
        [Tooltip("キャラクターの基準位置（例:  HMDやキャラクターのルート）")]
        private Transform _characterBase;
        
        [Header("Offset Settings")]
        [SerializeField]
        private Vector3 _handPositionOffset;
        
        [SerializeField]
        private Vector3 _handRotationOffset;
        
        [SerializeField]
        [Range(0f, 1f)]
        private float _positionSmoothing = 0.1f;

        // ===== Debug =====
        [Header("Debug")]
        [SerializeField]
        [Tooltip("オンにすると実行時の内部処理をログ出力します（開発用）。")]
        private bool _enableLogs;
        // =================

        [OdinSerialize]
        [ReadOnly]
        private ILeftTrackingPositionProvider _leftProvider;
        [OdinSerialize]
        [ReadOnly]
        private IRightTrackingPositionProvider _rightProvider;

        private IObjectResolver _resolver;

        [Inject]
        public void Construct(IObjectResolver resolver) {
            _resolver = resolver;
            if (_enableLogs) Debug.Log($"[HandIKController] Construct called. Resolver injected: {(resolver != null)}");
        }

        private void Start() {
            if (_resolver is not null) {
                _leftProvider = _resolver.Resolve<ILeftTrackingPositionProvider>();
                _rightProvider = _resolver.Resolve<IRightTrackingPositionProvider>();
                if (_enableLogs) Debug.Log($"[HandIKController] Start: Providers resolved. Left: {(_leftProvider != null)}, Right: {(_rightProvider != null)}");
            } else {
                if (_enableLogs) Debug.LogWarning("[HandIKController] Start: Resolver is null. Providers not resolved.");
            }
        }
            
        private void LateUpdate() {
            if (_leftProvider != null && _leftHandIKTarget != null) {
                if (_enableLogs) Debug.Log($"[HandIKController] LateUpdate: Updating left hand. ControllerPos: {_leftProvider.Provide()}");
                UpdateHandIK(_leftProvider.Provide(), _leftHandIKTarget);
            } else if (_enableLogs && (_leftProvider == null || _leftHandIKTarget == null)) {
                if (_leftProvider == null) Debug.LogWarning("[HandIKController] LateUpdate: Left provider is null.");
                if (_leftHandIKTarget == null) Debug.LogWarning("[HandIKController] LateUpdate: Left IK target is null.");
            }
            
            if (_rightProvider != null && _rightHandIKTarget != null) {
                if (_enableLogs) Debug.Log($"[HandIKController] LateUpdate: Updating right hand. ControllerPos: {_rightProvider.Provide()}");
                UpdateHandIK(_rightProvider. Provide(), _rightHandIKTarget);
            } else if (_enableLogs && (_rightProvider == null || _rightHandIKTarget == null)) {
                if (_rightProvider == null) Debug.LogWarning("[HandIKController] LateUpdate: Right provider is null.");
                if (_rightHandIKTarget == null) Debug.LogWarning("[HandIKController] LateUpdate: Right IK target is null.");
            }
        }
        
        private void UpdateHandIK(Vector3 controllerPosition, Transform ikTarget) {
            if (ikTarget == null) {
                if (_enableLogs) Debug.LogWarning("[HandIKController] UpdateHandIK: ikTarget is null. Aborting update.");
                return;
            }

            if (_enableLogs) Debug.Log($"[HandIKController] UpdateHandIK start. ControllerPos: {controllerPosition}, IKTarget: {ikTarget.name}");

            if (_characterBase == null) {
                // 基準位置がない場合は、コントローラーの位置をそのまま使用
                Vector3 targetPos = controllerPosition + _handPositionOffset;
                ikTarget.position = Vector3.Lerp(
                    ikTarget.position,
                    targetPos,
                    1f - _positionSmoothing
                );

                if (_enableLogs) Debug.Log($"[HandIKController] UpdateHandIK (no base): targetPos={targetPos}, afterLerp={ikTarget.position}");
                return;
            }
            
            // キャラクターの基準位置からの相対位置として計算
            Vector3 relativePosition = controllerPosition;
            Vector3 worldPosition = _characterBase.TransformPoint(relativePosition + _handPositionOffset);
            
            ikTarget.position = Vector3.Lerp(
                ikTarget. position,
                worldPosition,
                1f - _positionSmoothing
            );

            if (_enableLogs) Debug.Log($"[HandIKController] UpdateHandIK (with base): worldPosition={worldPosition}, afterLerp={ikTarget.position}");
        }
    }
}