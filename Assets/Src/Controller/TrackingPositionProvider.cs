using System;
using R3;
using RinaInput.Signal;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace Controller {
    
    /// <summary>
    /// ハンドコントローラのローカル座標を他のクラスに対して提供するクラスに対して約束するインターフェース
    /// </summary>
    public interface IHandTrackingPositionProvider : IStartable, IDisposable {

        Vector3 Provide();
    }
    
    /// <summary>
    /// ハンドコントローラーのローカル座標を他のクラスに対して提供するクラス
    /// </summary>
    /// <typeparam name="Hand"></typeparam>
    [Serializable]
    public abstract class ATrackingPositionProvider<Hand> : IHandTrackingPositionProvider where Hand : IHandTrackingModule {
        
        [SerializeField]
        [ReadOnly]
        private Vector3 _handPosition;

        [OdinSerialize]
        [ReadOnly]
        private Hand _trackingModule;
        
        private CompositeDisposable _disposables = new CompositeDisposable();

        private IObjectResolver _resolver;

        [Inject]
        protected ATrackingPositionProvider(IObjectResolver resolver) {
            _resolver = resolver;
        }

        public void Start() {
            if (_resolver is not null) {
                _trackingModule = _resolver.Resolve<Hand>();
            }
            if (_trackingModule is not null) {
                RegisterTrackingModule();
            }
        }

        public void Dispose() {
            _disposables?.Dispose();
            _disposables = null;
        }
        
        public Vector3 Provide() {
            if (_trackingModule is null) {
                return Vector3.zero;
            }
            return _handPosition;
        }

        
        private void RegisterTrackingModule() {
            _disposables?.Dispose();
            _disposables?.Clear();
            
            _disposables = new CompositeDisposable();
            
            _trackingModule ??= _resolver.Resolve<Hand>();
            if (_trackingModule is null) {
                return;
            }
            _trackingModule
                .Stream
                .Subscribe(OnPositionChanged)
                .AddTo(_disposables);
        }

        protected void OnPositionChanged(InputSignal<Vector3> signal) {
            _handPosition = signal.Value;
        }
    }

    /// <summary>
    /// 右手のローカル座標を他のクラスに対して提供するクラスに対して約束するインターフェース
    /// </summary>
    public interface IRightTrackingPositionProvider : IHandTrackingPositionProvider {
        
    }
    
    /// <summary>
    /// 右手のローカル座標を他のクラスに対して提供するクラス
    /// </summary>
    [Serializable]
    public class RightTrackingPositionProvider : ATrackingPositionProvider<IRightHandTrackingModule>, IRightTrackingPositionProvider {
        
        public RightTrackingPositionProvider(IObjectResolver resolver) : base(resolver) {}
    }

    /// <summary>
    /// 左手のローカル座標を他のクラスに対して提供するクラスに対して約束するインターフェース
    /// </summary>
    public interface ILeftTrackingPositionProvider : IHandTrackingPositionProvider {
        
    }
    
    /// <summary>
    /// 左手のローカル座標を他のクラスに対して提供するクラス
    /// </summary>
    [Serializable]
    public class LeftTrackingPositionProvider : ATrackingPositionProvider<ILeftHandTrackingModule>, ILeftTrackingPositionProvider {
        
        public LeftTrackingPositionProvider(IObjectResolver resolver) : base(resolver) {}
    }
}