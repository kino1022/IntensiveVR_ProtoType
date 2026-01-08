using UnityEngine;

namespace Controller {
    
    public interface IRightHandTrackingModule : IHandTrackingModule {
        
    }
    
    [CreateAssetMenu(menuName = "Project/Controller/RightHand")]
    public class RightHandTrackingModule : HandTrackingModule, IRightHandTrackingModule {
        
    }
}