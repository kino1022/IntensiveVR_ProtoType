using RinaInput.Controller.Module;
using UnityEngine;

namespace Controller {

    public interface IHandTrackingModule : IInputModule<Vector3> {
        
    }
    
    public class HandTrackingModule : PositionModule, IHandTrackingModule {
        
        
    }
}