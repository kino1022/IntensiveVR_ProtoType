using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using VContainer;

namespace Controller.Hand {
    [Serializable]
    public abstract class AActionBasedHandTrackingModule : IHandTrackingModule {
        
        [SerializeField]
        [ReadOnly]
        private Vector3 _currentPos = Vector3.zero;
        
        [SerializeField]
        [ReadOnly]
        private Quaternion _currentRot = Quaternion.identity;
        
        [SerializeField]
        [ReadOnly]
        private bool _isEnable = true;
        
        private InputActionProperty _cachedPositionAction;
        
        private InputActionProperty _cachedRotationAction;

        private IHandModuleInputReferenceConfig _inputReference;
        
        [SerializeField]
        [ReadOnly]
        private bool _isCachedInputActionProperty = false;
        
        private XRNode _cachedNode;

        private IObjectResolver _resolver;
        
        public Vector3 CurrentPosition => _currentPos;
        
        public Quaternion CurrentRotation => _currentRot;
        
        public bool IsEnable => _isEnable || _isCachedInputActionProperty;
        
        protected AActionBasedHandTrackingModule(XRNode node, IObjectResolver resolver) {
            _cachedNode = node;
            _resolver = resolver;
        }

        public void Start() {
            if (_resolver is not null) {
                _inputReference = _resolver.Resolve<IHandModuleInputReferenceConfig>();
            }
            CachedInputActionProperty();
        }

        public void Tick() {
            if (_inputReference is not null) {
                //参照をキャッシュしていないのならキャッシュする
                if (_isCachedInputActionProperty is false) {
                    CachedInputActionProperty();
                }
            }
            UpdateTrackingData();
        }

        public void Dispose() {
            
        }

        protected void CachedInputActionProperty() {

            if (_inputReference is null) {
                Debug.LogError($"{GetType().Name}: InputActionProperty: InputReference is null.");
                _isCachedInputActionProperty = false;
                return;
            }
            
            if (_cachedNode is XRNode.RightHand) {
                _cachedPositionAction = _inputReference.RightPositionAction;
                _cachedRotationAction = _inputReference.RightRotationAction;
            }
            else if (_cachedNode is XRNode.LeftHand) {
                _cachedPositionAction = _inputReference.LeftPositionAction;
                _cachedRotationAction = _inputReference.LeftRotationAction;
            }
            _isCachedInputActionProperty = true;
        }

        protected void UpdateTrackingData() {
            if (_isCachedInputActionProperty is false) {
                Debug.LogError($"{GetType().Name}: UpdateTrackingData: InputActionProperty is not cached.");
                return;
            }

            if (_cachedPositionAction.action is not null) {
                _currentPos = _cachedPositionAction.action.ReadValue<Vector3>();
            }
            if (_cachedRotationAction.action is not null) {
                _currentRot = _cachedRotationAction.action.ReadValue<Quaternion>();
            }
        }
    }
    
    public class RightHandActionBasedTrackingModule : AActionBasedHandTrackingModule, IRightHandTrackingModule {
        public RightHandActionBasedTrackingModule(IObjectResolver resolver) : base(XRNode.RightHand, resolver) {
        }
    }
    
    public class LeftHandActionBasedTrackingModule : AActionBasedHandTrackingModule, ILeftHandTrackingModule {
        public LeftHandActionBasedTrackingModule(IObjectResolver resolver) : base(XRNode.LeftHand, resolver) {
        }
    }
}