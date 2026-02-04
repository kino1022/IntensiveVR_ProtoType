using Cam;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using VContainer;

namespace Player.Action {
    
    public class WalkAction : ReferenceBasedActionBehaviour<Vector2> {
        
        [Title("Dependencies")]
        [SerializeField]
        [LabelText("CharacterController")]
        protected CharacterController _characterController;

        [Title("RuntimeDependencies")]
        
        [OdinSerialize]
        [LabelText("歩行速度マネージャ")]
        [ReadOnly]
        private IWalkSpeedProvider _speedProvider;

        [OdinSerialize]
        [LabelText("XRCamera")]
        [ReadOnly]
        private IXROriginIdentifiedCamera _originCamera;
        
        private IObjectResolver _resolver;
        
        [Title("WalkActionStatus")]
        [SerializeField]
        [LabelText("現在の歩行方向")]
        [ReadOnly]
        private Vector3 _currentDirection = Vector3.zero;

        [Inject]
        public void Construct(IObjectResolver resolver) {
            _resolver = resolver;
        }

        protected override void OnStart() {
            base.OnStart();
            if (_resolver is not null) {
                _speedProvider = _resolver.Resolve<IWalkSpeedProvider>();
                _originCamera = _resolver.Resolve<IXROriginIdentifiedCamera>();
            }
        }

        public override void StartAction(ref ActionContext<Vector2> context) {
            
        }

        public override void PerformAction(ref ActionContext<Vector2> context) {
            _currentDirection = CalculateDirection(ref context);
            ExecuteMove(ref context);
        }
        
        public override void CancelAction(ref ActionContext<Vector2> context) {
            _currentDirection = Vector3.zero;
        }

        private void ExecuteMove(ref ActionContext<Vector2> context) {
            _speedProvider ??= _resolver.Resolve<IWalkSpeedProvider>();
            _speedProvider ??= gameObject.GetComponentInChildren<IWalkSpeedProvider>();

            _characterController ??= gameObject.GetComponentInChildren<CharacterController>();

            _characterController.Move(_currentDirection * _speedProvider.GetWalkSpeed() * Time.deltaTime);
        }

        private Vector3 CalculateDirection(ref ActionContext<Vector2> context) {

            _originCamera ??= _resolver.Resolve<IXROriginIdentifiedCamera>();

            if (_originCamera is null) {
                return new  Vector3(context.Value.x, 0.0f, context.Value.y);
            }
            
            var camForward = _originCamera.IdentifiedCamera.transform.forward;
            camForward.y = 0.0f;
            camForward.Normalize();
            
            var camRight = _originCamera.IdentifiedCamera.transform.right;
            camRight.y = 0.0f;
            camRight.Normalize();
            
            return camForward * context.Value.y + camRight * context.Value.x;
        }
    }
}