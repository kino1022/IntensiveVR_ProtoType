using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using VContainer;

namespace Player.Action {
    
    public class WalkAction : ModuleBasedActionBehaviour<Vector2> {
        
        [Title("Dependencies")]
        [SerializeField]
        [LabelText("CharacterController")]
        protected CharacterController _characterController;

        [Title("RuntimeDependencies")]
        
        [OdinSerialize]
        [LabelText("歩行速度マネージャ")]
        [ReadOnly]
        private IWalkSpeedProvider _speedProvider;

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
            }
        }

        public override void StartAction(ref ActionContext<Vector2> context) {
            
        }

        public override void PerformAction(ref ActionContext<Vector2> context) {
            _currentDirection = context.Value.normalized;
            ExecuteMove();
        }
        
        public override void CancelAction(ref ActionContext<Vector2> context) {
            _currentDirection = Vector3.zero;
        }

        private void ExecuteMove() {
            _speedProvider ??= _resolver.Resolve<IWalkSpeedProvider>();
            _speedProvider ??= gameObject.GetComponentInChildren<IWalkSpeedProvider>();

            _characterController ??= gameObject.GetComponentInChildren<CharacterController>();

            _characterController.Move(_currentDirection * _speedProvider.GetWalkSpeed() * Time.deltaTime);
        }
    }
}