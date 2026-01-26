using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.XR;
using VContainer.Unity;

namespace Controller.Hand {

    public interface IHandTrackingModule : IStartable, ITickable, IDisposable {
        
        /// <summary>
        /// 現在のローカル座標
        /// </summary>
        Vector3 CurrentPosition { get; }
        
        /// <summary>
        /// 現在のローカル回転
        /// </summary>
        Quaternion CurrentRotation { get; }
        
        /// <summary>
        /// 有効かされているかどうか
        /// </summary>
        bool IsEnable { get; }
        
    }
    
    [Serializable]
    public abstract class AHandTrackingModule : IHandTrackingModule {

        [SerializeField]
        [LabelText("現在のローカル位置")]
        [ReadOnly]
        protected Vector3 _currntPos = Vector3.zero;
        
        [SerializeField]
        [LabelText("現在のローカル回転")]
        [ReadOnly]
        protected Quaternion _currntRot = Quaternion.identity;

        [SerializeField]
        [LabelText("有効状態")]
        [ReadOnly]
        protected bool _isEnable = false;
        
        [SerializeField]
        [LabelText("デバッグログ有効化")]
        private bool _enableDebugLogs = false;

        protected InputDevice _trackingDevice;
        
        protected XRNode _cachedXRNode;
        
        public Vector3 CurrentPosition => _currntPos;
        
        public Quaternion CurrentRotation => _currntRot;

        public bool IsEnable =>  _isEnable;

        protected AHandTrackingModule(XRNode handType) {
            _cachedXRNode = handType;
            _isEnable = GetTrackingDevice(ref _trackingDevice);
        }

        protected bool GetTrackingDevice(ref InputDevice deviceRef) {
            deviceRef = InputDevices.GetDeviceAtXRNode(_cachedXRNode);
            if (deviceRef.isValid) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// モジュールから座標を取得して更新する
        /// </summary>
        protected void UpdatePosition() {
            if (_trackingDevice.isValid) {
                if (_trackingDevice.TryGetFeatureValue(CommonUsages.devicePosition, out _currntPos)) {
                    if (_enableDebugLogs) {
                        Debug.Log($"{GetType().Name}.{nameof(UpdatePosition)}: {_currntPos.x}, {_currntPos.y}, {_currntPos.z}");
                    }
                }
            }
            else {
                Debug.LogError($"{GetType().Name}/UpdatePosition: Device is not valid.");
            }
        }

        /// <summary>
        /// モジュールからローカル回転を取得して更新する
        /// </summary>
        protected void UpdateRotation() {
            if (_trackingDevice.isValid) {
                if (_trackingDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out _currntRot)) {
                    if (_enableDebugLogs) {
                        Debug.Log($"{GetType().Name}.{nameof(UpdateRotation)}: {_currntRot.x}, {_currntRot.y}, {_currntRot.z}, {_currntRot.w}");
                    }
                }
            }
            else {
                Debug.LogError($"{GetType().Name}/UpdateRotation: Device is not valid.");
            }
        }

        public void Start() {
            if (_enableDebugLogs) {
                Debug.Log($"{GetType().Name}/{nameof(Start)}: Starting Hand Tracking Module for {_cachedXRNode}");
            }
            //取得されていなかった場合は再取得を試みる
            if (!_isEnable) {
                _isEnable = GetTrackingDevice(ref _trackingDevice);
            }
        }

        public void Tick() {
            if (_enableDebugLogs) {
                Debug.Log($"{GetType().Name}/{nameof(Tick)}: Updating Hand Tracking Module for {_cachedXRNode}");
            }
            //取得されていなかった場合は再取得を試みる
            if (!_isEnable) {
                _isEnable = GetTrackingDevice(ref _trackingDevice);
            }
            //再取得後に有効、もしくは元々有効であれば座標を更新
            if (_isEnable) {
                UpdatePosition();
                UpdateRotation();
            }
        }

        public void Dispose() {
            if (_enableDebugLogs) {
                Debug.Log($"{GetType().Name}/{nameof(Dispose)}: Disposing Hand Tracking Module for {_cachedXRNode}");
            }
            _isEnable = false;
        }
    }
    
    public interface IRightHandTrackingModule : IHandTrackingModule {
    }
    
    public class RightHandTrackingModule : AHandTrackingModule, IRightHandTrackingModule {
        public RightHandTrackingModule() : base(XRNode.RightHand) {
        }
    }
    
    public interface ILeftHandTrackingModule : IHandTrackingModule {
    }
    
    public class LeftHandTrackingModule : AHandTrackingModule, ILeftHandTrackingModule {
        public LeftHandTrackingModule() : base(XRNode.LeftHand) {
        }
    }
}