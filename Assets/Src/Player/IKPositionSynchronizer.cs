using System;
using Controller;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using VContainer;

namespace Player {
    public class IKPositionSynchronizer : SerializedMonoBehaviour {

        [SerializeField]
        [LabelText("右手IKターゲット")]
        private Transform _rightHandIKTarget;
        
        [SerializeField]
        [LabelText("左手IKターゲット")]
        private Transform _leftHandIKTarget;
        
        [OdinSerialize]
        [LabelText("右手位置")]
        [ReadOnly]
        private IRightTrackingPositionProvider _rightProvider;
        
        [OdinSerialize]
        [LabelText("左手位置")]
        [ReadOnly]
        private ILeftTrackingPositionProvider _leftProvider;

        private IObjectResolver _resolver;

        [Inject]
        public void Construct(IObjectResolver resolver) {
            _resolver = resolver;
        }

        private void Start() {
            if (_resolver is not null) {
                _rightProvider = _resolver.Resolve<IRightTrackingPositionProvider>();
                _leftProvider = _resolver.Resolve<ILeftTrackingPositionProvider>();
            }
        }

        private void LateUpdate() {
            SynchronizeHandPositions();
        }
        
        private void SynchronizeHandPositions() {
            
            _rightProvider ??= _resolver.Resolve<IRightTrackingPositionProvider>();
            _leftProvider ??= _resolver.Resolve<ILeftTrackingPositionProvider>();
            
            if (_rightHandIKTarget is not null && _rightProvider is not null) {
                _rightHandIKTarget.position = _rightHandIKTarget.TransformPoint(_rightProvider.Provide());
            }
            if (_leftHandIKTarget is not null && _leftProvider is not null) {
                _leftHandIKTarget.position = _leftHandIKTarget.TransformPoint(_leftProvider.Provide());
            }
            
        }
    }
}