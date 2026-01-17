using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.XR;

namespace Player {
    /// <summary>
    /// XRと操作キャラクターのローカル座標をベースにしてIKターゲットの操作を行うクラス
    /// </summary>
    public class LocalBasedHandIKController : SerializedMonoBehaviour {
        
        [Title("IKターゲット設定")]
        
        [SerializeField]
        [LabelText("右手IK対象")]
        private Transform _rightHandIKTarget;
        
        [SerializeField]
        [LabelText("左手IK対象")]
        private Transform _leftHandIKTarget;

        [SerializeField]
        [LabelText("基準Transform")]
        private Transform _baseTransform;

        [Title("Status")]

        [SerializeField]
        [LabelText("IK同期速度")]
        [Range(0.1f, 10.0f)]
        private float _ikSmoothSpeed = 1.0f;
        
        [SerializeField]
        [LabelText("位置オフセット")]
        private Vector3 _positionOffset;
        
        [SerializeField]
        [LabelText("回転オフセット")]
        private Vector3 _rotationOffsetEuler;
        
        [Title("Cached Input Data")]

        [SerializeField]
        [LabelText("右手ローカル位置（デバッグ用）")]
        [ReadOnly]
        private Vector3 _cachedRightLocalPosition;
        
        [SerializeField]
        [LabelText("左手ローカル位置（デバッグ用）")]
        [ReadOnly]
        private Vector3 _cachedLeftLocalPosition;
        
        private Quaternion _cachedRightLocalRotation = Quaternion.identity;
        
        private Quaternion _cachedLeftLocalRotation = Quaternion.identity;
        
        private InputDevice _cachedRightDevice;
        
        private InputDevice _cachedLeftDevice;

        [SerializeField]
        [LabelText("デバッグログを有効化")]
        private bool _enableDebugLogs = true;

        private void Start() {
            CachedInputDevices(
                XRNode.RightHand,
                ref _cachedRightDevice
                );
            CachedInputDevices(
                XRNode.LeftHand,
                ref _cachedLeftDevice
                );
        }

        private void Update() {
            UpdateCachedInputData();
        }

        private void LateUpdate() {
            UpdateIKTargets(
                _rightHandIKTarget,
                _cachedRightLocalPosition,
                _cachedRightLocalRotation
                );
            UpdateIKTargets(
                _leftHandIKTarget, 
                _cachedLeftLocalPosition, 
                _cachedLeftLocalRotation
                );
        }

        /// <summary>
        /// 入力デバイスの入力情報を一括キャッシュする処理
        /// </summary>
        private void UpdateCachedInputData() {
            CacheInputPosition(
                _cachedRightDevice, 
                ref _cachedRightLocalPosition
                );
            CacheInputRotation(
                _cachedRightDevice, 
                ref _cachedRightLocalRotation
                );
            CacheInputPosition(
                _cachedLeftDevice,
                ref _cachedLeftLocalPosition
                );
            CacheInputRotation(
                _cachedLeftDevice,
                ref _cachedLeftLocalRotation
                );
        }

        /// <summary>
        /// 引数として渡された入力デバイスのローカル位置をキャッシュする
        /// </summary>
        /// <param name="device"></param>
        /// <param name="pos"></param>
        private void CacheInputPosition(InputDevice device, ref Vector3 pos) {
            if (device.isValid) {
                if (device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 devicePos)) {
                    pos = devicePos;
                    if (_enableDebugLogs) {
                        Debug.Log($"{GetType().Name}/CacheInputPosition: Cached Position for Device {device.name}: {pos}");
                    }
                }
            }
        }
        
        
        /// <summary>
        /// 引数として渡された入力デバイスの回転をキャッシュする
        /// </summary>
        /// <param name="device"></param>
        /// <param name="rot"></param>
        private void CacheInputRotation(InputDevice device, ref Quaternion rot) {
            if (device.isValid) {
                if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion deviceRot)) {
                    rot = deviceRot;
                    if (_enableDebugLogs) {
                        Debug.Log($"{GetType().Name}/CacheInputRotation: Cached Rotation for Device {device.name}: {rot.eulerAngles}");
                    }
                }
            }
        }
        
        /// <summary>
        /// 入力デバイスの参照をキャッシュする
        /// </summary>
        /// <param name="node"></param>
        /// <param name="cachedDevice"></param>
        private void CachedInputDevices(XRNode node, ref InputDevice cachedDevice) {
            cachedDevice = InputDevices.GetDeviceAtXRNode(node);
        }

        /// <summary>
        /// IKターゲットの位置と回転を更新する
        /// </summary>
        /// <param name="target"></param>
        /// <param name="localPos"></param>
        /// <param name="localRot"></param>
        private void UpdateIKTargets(Transform target, Vector3 localPos, Quaternion localRot) {

            if (_baseTransform is null) {
                Debug.LogError($"{GetType().Name}/UpdateIKTargets: Base Transform is null.");
                return;
            }
            
            if (target is not null) {

                var desiredPos = _baseTransform.TransformPoint(localPos + _positionOffset); 
                var desiredRot = _baseTransform.rotation * localRot * Quaternion.Euler(_rotationOffsetEuler);
                
                float t = Mathf.Clamp01(_ikSmoothSpeed * Time.deltaTime);
                
                target.position = Vector3.Lerp(
                    target.position,
                    desiredPos,
                    t
                );
                
                target.rotation = Quaternion.Slerp(
                    target.rotation,
                    desiredRot,
                    t
                );
            }
        }
    }
}