using System;
using R3;
using RinaInput.Controller.Module;
using RinaInput.Provider;
using RinaInput.Signal;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

namespace Controller {

    public interface IHandTrackingModule : IInputModule<Vector3> {
        Transform GetControllerTransform();
    }
    
    public class HandTrackingModule : ScriptableObject, IHandTrackingModule {
        
        #region old source
        /*
        [SerializeField]
        [LabelText("入力ソース")]
        private InputActionProperty m_actionProp;
        
        private ReactiveProperty<bool> m_isEnable = new(true);
        
        private Observable<InputSignal<Vector3> > m_stream;
        
        private CompositeDisposable _dummyDisposable = new CompositeDisposable();
        
        public ReadOnlyReactiveProperty<bool> IsEnable => m_isEnable;
        
        public Observable<InputSignal<Vector3>> Stream => m_stream != null
            ? m_stream.Where(_ => m_isEnable.CurrentValue == true)
            : Observable.Empty<InputSignal<Vector3>>();
        
        public void Start() {
            m_actionProp.action?.Enable();
        }

        public void ChangeEnable(bool isEnable) {
            m_isEnable.Value = isEnable;
        }

        public void GenerateStream(IInputStreamProvider provider) {
            if (provider is  null) {
                return;
            }
            
            m_stream = provider
                .GetStream<Vector3>(m_actionProp.reference) ?? throw new NullReferenceException("Stream is null");
            
            Stream
                .Subscribe(_ => {
                    Debug.Log($"{GetType().Name}:　Received Input Signal: {_.Value}");
                })
                .AddTo(_dummyDisposable);
        }
        */
        #endregion
        
        #region old source 2
        /*
        [SerializeField]
        [LabelText("追跡対象コントローラー")]
        private TrackedPoseDriver _handController;
        
        private ReactiveProperty<bool> m_isEnable = new(true);
        
        private Observable<InputSignal<Vector3> > m_stream;
        
        private CompositeDisposable _disposable = new CompositeDisposable();
        
        public ReadOnlyReactiveProperty<bool> IsEnable => m_isEnable;
        
        public Observable<InputSignal<Vector3>> Stream => m_stream != null
            ? m_stream.Where(_ => m_isEnable.CurrentValue == true)
            : Observable.Empty<InputSignal<Vector3>>();

        public void Start() {
            _handController?
                .positionInput
                .reference
                .action
                .Enable();
        }
        
        public void ChangeEnable(bool isEnable) {
            m_isEnable.Value = isEnable;
        }

        public void GenerateStream(IInputStreamProvider provider) {
            if (_handController is null) {
                throw new NullReferenceException("Hand Controller is null");
            }
            
            m_stream = provider
                .GetStream<Vector3>(_handController.positionInput.reference) 
                ?? throw new NullReferenceException("Stream is null");

            m_stream
                .Subscribe(x => {
                    Debug.Log($"{GetType().Name}: Received Input Signal: {x.Value}");
                })
                .AddTo(_disposable);
        }
        */
        #endregion
        
        [SerializeField]
        [LabelText("追跡対象コントローラー")]
        private TrackedPoseDriver _handController;
        
        private ReactiveProperty<bool> m_isEnable = new(true);
        
        private Observable<InputSignal<Vector3> > m_stream;
        
        private CompositeDisposable _disposable = new CompositeDisposable();
        
        public ReadOnlyReactiveProperty<bool> IsEnable => m_isEnable;
        
        public Observable<InputSignal<Vector3>> Stream => m_stream != null
            ? m_stream.Where(_ => m_isEnable.CurrentValue == true)
            : Observable.Empty<InputSignal<Vector3>>();

        public Transform GetControllerTransform() {
            return _handController?.transform;
        }

        public void Start() {
            _handController? 
                .positionInput
                .reference
                .action
                .Enable();
        }
        
        public void ChangeEnable(bool isEnable) {
            m_isEnable.Value = isEnable;
        }

        public void GenerateStream(IInputStreamProvider provider) {
            if (_handController is null) {
                throw new NullReferenceException("Hand Controller is null");
            }
            
            m_stream = provider
                . GetStream<Vector3>(_handController.positionInput.reference) 
                ?? throw new NullReferenceException("Stream is null");

            m_stream
                .Subscribe(x => {
                    Debug.Log($"{GetType().Name}: Received Input Signal: {x.Value}");
                })
                .AddTo(_disposable);
        }
    }
}