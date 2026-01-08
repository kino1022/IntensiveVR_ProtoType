using UnityEngine;

namespace Controller {
    
    public interface ILeftHandTrackingModule : IHandTrackingModule {
        
    }
    
    [CreateAssetMenu(menuName = "Project/Controller/LeftHand")]
    public class LeftHandTrackingModule : HandTrackingModule, ILeftHandTrackingModule {
        
    }
}