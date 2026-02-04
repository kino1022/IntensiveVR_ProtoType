using Grab.Item;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grab {

    public readonly struct GrabTargetContext {
        
        public readonly GrabableBehaviour Target { get; }
        
        public readonly bool ExsistTarget { get; }

        public GrabTargetContext(GrabableBehaviour target, bool exsist) {
            Target = target;
            ExsistTarget = exsist;
        }
    }

    public interface IGrabTargetProvider {

        bool TryGetGrabTarget(out GrabTargetContext context);
        
    }
    
    public class GrabTargetManager : SerializedMonoBehaviour, IGrabTargetProvider {
        
        [Title("Config")]

        [SerializeField]
        [LabelText("インタラクト距離")]
        [Range(0.0f, 100.0f)]
        private float _limitRange = 0.0f;
        
        private RaycastHit _raycastHits;

        public bool TryGetGrabTarget(out GrabTargetContext context) {
            TryGetRaycastHits();
            var grabable = _raycastHits.collider?.GetComponent<GrabableBehaviour>();
            if (grabable is null) {
                context = default;
                return false;
            }
            context = new GrabTargetContext(grabable, true);
            return true;
        }
        
        public void TryGetRaycastHits() {
            Physics.Raycast(
                transform.position,
                transform.forward.normalized,
                out _raycastHits,
                _limitRange
            );
        }
        
    }
}