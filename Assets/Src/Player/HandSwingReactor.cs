using Controller;
using R3;
using RinaInput.Controller.Module;
using RinaInput.Operators.Position;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using VContainer;

namespace Player {
    public class HandSwingReactor : SerializedMonoBehaviour {

        [SerializeField]
        [LabelText("右手の攻撃判定")]
        private Collider _rightCollider;
        
        [SerializeField]
        [LabelText("左手の攻撃判定")]
        private Collider _leftCollider;
        
        [SerializeField]
        [LabelText("スイング判定閾値")]
        private float _swingThreshold = 1.0f;

        [OdinSerialize]
        [ReadOnly]
        private IRightHandTrackingModule _rightHand;
        
        [OdinSerialize]
        [ReadOnly]
        private ILeftHandTrackingModule _leftHand;

        [SerializeField]
        [ReadOnly]
        private bool _registeredRightHand = false;
        
        [SerializeField]
        [ReadOnly]
        private bool _registeredLeftHand = false;

        private IObjectResolver _resolver;
        
        [Inject]
        public void Construct (IObjectResolver resolver) {
            _resolver = resolver;
        }

        private void Start() {
            if (_resolver is not null) {
                _rightHand = _resolver.Resolve<IRightHandTrackingModule>();
                _leftHand = _resolver.Resolve<ILeftHandTrackingModule>();
            }
            
            //右腕の動きを購読
            RegisterHandSwing(_rightHand, _rightCollider, ref _registeredRightHand);
            //左腕の動きを購読
            RegisterHandSwing(_leftHand, _leftCollider, ref _registeredLeftHand);
        }
        
        
        private void RegisterHandSwing (IInputModule<Vector3> module, Collider collider, ref bool registeredFlag) {
            if (collider is null || module is null) {
                return;
            }
            if (registeredFlag) {
                return;
            }
            
            module
                .OnSwing(_swingThreshold)
                .Subscribe(_ => {
                    OnSwing(collider);
                })
                .AddTo(this);
            registeredFlag = true;
        }

        private void OnSwing(Collider collider) {
            
        }
    }
}