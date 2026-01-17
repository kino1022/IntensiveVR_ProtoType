using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller.Hand {
    
    public interface IHandModuleInputReferenceConfig {
        
        InputActionProperty RightPositionAction { get; }
        
        InputActionProperty RightRotationAction { get; }
        
        InputActionProperty LeftPositionAction { get; }
        
        InputActionProperty LeftRotationAction { get; }
        
    }
    
    [CreateAssetMenu(menuName = "Project/Controller/HandModuleInputReferenceConfig")]
    public class HandModuleInputReferenceConfig : SerializedScriptableObject, IHandModuleInputReferenceConfig{
        
        [SerializeField]
        public InputActionProperty RightPositionAction;
        
        [SerializeField]
        public InputActionProperty RightRotationAction;
        
        [SerializeField]
        public InputActionProperty LeftPositionAction;
        
        [SerializeField]
        public InputActionProperty LeftRotationAction;
        
        InputActionProperty IHandModuleInputReferenceConfig.RightPositionAction => RightPositionAction;
        
        InputActionProperty IHandModuleInputReferenceConfig.RightRotationAction => RightRotationAction;
        
        InputActionProperty IHandModuleInputReferenceConfig.LeftPositionAction => LeftPositionAction;
        
        InputActionProperty IHandModuleInputReferenceConfig.LeftRotationAction => LeftRotationAction;
        
    }
}