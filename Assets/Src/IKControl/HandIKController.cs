using Controller;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine. Animations. Rigging;
using VContainer;

namespace Player {
    /// <summary>
    /// TrackingModuleから取得したコントローラー位置と
    /// キャラクターの基準位置の差分を使ってIKターゲットを制御
    /// </summary>
    public class HandIKController :  SerializedMonoBehaviour {
        
        [Header("IK Targets")]
        [SerializeField]
        private RigTransform _leftHandIKTarget;
        
        [SerializeField]
        private RigTransform _rightHandIKTarget;
        
        [Header("Controller References")]
        [SerializeField]
        [Tooltip("左コントローラーのTransform（TrackedPoseDriverがアタッチされているGameObject）")]
        private Transform _leftControllerTransform;
        
        [SerializeField]
        [Tooltip("右コントローラーのTransform（TrackedPoseDriverがアタッチされているGameObject）")]
        private Transform _rightControllerTransform;
        
        [Header("Offset Settings")]
        [SerializeField]
        [Tooltip("手の位置の微調整用オフセット（ワールド座標）")]
        private Vector3 _handPositionOffset;
        
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
        private ILeftHandTrackingModule _leftModule;
        
        [OdinSerialize]
        [ReadOnly]
        private IRightHandTrackingModule _rightModule;

        private IObjectResolver _resolver;

        [Inject]
        public void Construct(IObjectResolver resolver) {
            _resolver = resolver;
            if (_enableLogs) Debug.Log($"[HandIKController] Construct called.  Resolver injected: {(resolver != null)}");
        }

        private void Start() {
            if (_resolver is not null) {
                _leftModule = _resolver.Resolve<ILeftHandTrackingModule>();
                _rightModule = _resolver.Resolve<IRightHandTrackingModule>();
                
                // TrackedPoseDriverのTransformを取得
                if (_leftControllerTransform == null && _leftModule != null) {
                    _leftControllerTransform = _leftModule.GetControllerTransform();
                }
                if (_rightControllerTransform == null && _rightModule != null) {
                    _rightControllerTransform = _rightModule.GetControllerTransform();
                }
                
                if (_enableLogs) Debug.Log($"[HandIKController] Start:  Controllers resolved.  Left: {(_leftControllerTransform != null)}, Right: {(_rightControllerTransform != null)}");
            } else {
                if (_enableLogs) Debug.LogWarning("[HandIKController] Start:  Resolver is null.");
            }
        }
            
        private void LateUpdate() {
            if (_leftControllerTransform != null && _leftHandIKTarget != null) {
                if (_enableLogs) Debug.Log($"[HandIKController] LateUpdate:  Updating left hand.  ControllerWorldPos: {_leftControllerTransform.position}");
                UpdateHandIK(_leftControllerTransform. position, _leftHandIKTarget);
            }
            
            if (_rightControllerTransform != null && _rightHandIKTarget != null) {
                if (_enableLogs) Debug.Log($"[HandIKController] LateUpdate: Updating right hand. ControllerWorldPos: {_rightControllerTransform.position}");
                UpdateHandIK(_rightControllerTransform.position, _rightHandIKTarget);
            }
        }
        
        private void UpdateHandIK(Vector3 controllerWorldPosition, RigTransform ikTarget) {
            if (ikTarget == null) {
                if (_enableLogs) Debug.LogWarning("[HandIKController] UpdateHandIK: ikTarget is null. Aborting update.");
                return;
            }

            if (_enableLogs) Debug.Log($"[HandIKController] UpdateHandIK: ControllerWorldPos: {controllerWorldPosition}, IKTarget: {ikTarget.name}");

            // オフセットを適用
            Vector3 targetPosition = controllerWorldPosition + _handPositionOffset;
            
            // スムージングを適用
            ikTarget.transform.position = Vector3.Lerp(
                ikTarget.transform.position,
                targetPosition,
                1f - _positionSmoothing
            );

            if (_enableLogs) Debug.Log($"[HandIKController] UpdateHandIK: targetPos={targetPosition}, afterLerp={ikTarget.transform.position}");
        }
    }
}