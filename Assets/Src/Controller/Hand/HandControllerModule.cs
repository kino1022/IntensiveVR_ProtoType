using System;
using R3;
using RinaInput.Controller.Module;
using RinaInput.Provider;
using RinaInput.Signal;
using UnityEngine;
using UnityEngine.XR;
using VContainer.Unity;

namespace Controller.Hand {

    public interface IHandControllerModule : IInputModule<HandControllerData> {
        
        Vector3 CurrentLocalPosition { get; }
        
        Quaternion CurrentLocalRotation { get; }
        
    }
    
    public struct HandControllerData {
        
        public Vector3 Position;
        
        public Quaternion Rotation;
        
    }
    
    public abstract class AHandControllerModule : ITickable, IStartable, IDisposable {

        private ReactiveProperty<bool> _isEnable = new(true);

        private Vector3 _currentPosition;
        
        private Quaternion _currentRotation;

        private Observable<Vector3> _positionStream;
        
        private Observable<Quaternion> _rotationStream;

        private InputDevice _device;
        
        private CompositeDisposable _disposables = new();
        
        public Observable<Vector3> PositionStream => _positionStream is not null ?
            _positionStream.Where(_ => _isEnable.CurrentValue) :
            Observable.Empty<Vector3>();
        
        public Observable<Quaternion> RotationStream => _rotationStream is not null ?
            _rotationStream.Where(_ => _isEnable.CurrentValue) :
            Observable.Empty<Quaternion>();
        

        public ReadOnlyReactiveProperty<bool> IsEnable => _isEnable;

        protected AHandControllerModule(XRNode node) {
            var devices = InputDevices.GetDeviceAtXRNode(node);
            _device = devices;
        }

        public void Start() {
            _positionStream = CreateStream(() => _currentPosition);
            _rotationStream = CreateStream(() => _currentRotation);
        }

        public void Tick() {
            if (_device.isValid) {
                //位置の更新処理
                if (_device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 devicePos)) {
                    _currentPosition = devicePos;
                }
                //回転の更新処理
                if (_device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion deviceRot)) {
                    _currentRotation = deviceRot;
                }
            }
        }

        public void Dispose() {
            
        }

        private Observable<T> CreateStream<T>(Func<T> valueFunc) where T : struct {
            var observer = Observable
                .EveryValueChanged(valueFunc, x => x)
                .DistinctUntilChanged()
                .Select(_ => valueFunc());
            return observer;
        }
    }
}