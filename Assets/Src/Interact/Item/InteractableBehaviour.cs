using UnityEngine;
using VContainer;

namespace Interact.Item {

    public interface IInteractable {
        
    }

    public readonly struct InteractContext {

        public readonly Transform InteractTransform;
        
        public readonly GameObject Interactor;

        public readonly IObjectResolver Resolver;
        
        public InteractContext(Transform interactTransform, GameObject interactor, IObjectResolver resolver) {
            InteractTransform = interactTransform;
            Interactor = interactor;
            Resolver = resolver;
        }
        
    }
    
    public class InteractableBehaviour {
        
    }
}