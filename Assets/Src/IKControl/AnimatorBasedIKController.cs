using Controller.Hand;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using VContainer;

namespace Player {
    public class AnimatorBasedIKController : SerializedMonoBehaviour {
        
        [Title("Dependencies")]
        
        [SerializeField]
        [LabelText("右手IKコンストレイント")]
        private TwoBoneIKConstraint _rightHandIK;
        
        [SerializeField]
        [LabelText("左手IKコンストレイント")]
        private TwoBoneIKConstraint _leftHandIK;
        
        [SerializeField]
        [LabelText("右手オブジェクト")]
        private Transform _rightHandObject;
        
        [SerializeField]
        [LabelText("左手オブジェクト")]
        private Transform _leftHandObject;
        
        private IObjectResolver _resolver;
        
        [Inject]
        public void Construct(IObjectResolver resolver) {
            _resolver = resolver;
        }

        private void Start() {
            SetTarget(_rightHandObject, _rightHandIK);
            SetTarget(_leftHandObject, _leftHandIK);
        }

        private void SetTarget(Transform target, TwoBoneIKConstraint constraint) {
            if (target is null) {
                return;
            }
            if (constraint is null) {
                return;
            }
            constraint.data.target = target;
        }
    }
}