using Controller.Hand;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using VContainer;
using VContainer.Unity;

namespace Controller {
    public class ControllerInstaller : SerializedMonoBehaviour, IInstaller {
        
        [OdinSerialize]
        private IHandModuleInputReferenceConfig _handModuleInputReferenceConfig;
        
        [OdinSerialize]
        [Inject]
        private IRightHandTrackingModule _rightHandTrackingModule;
        
        [OdinSerialize]
        [Inject]
        private ILeftHandTrackingModule _leftHandTrackingModule;

        public void Install(IContainerBuilder builder) {

            if (_handModuleInputReferenceConfig is not null) {
                builder
                    .RegisterInstance(_handModuleInputReferenceConfig)
                    .As(typeof(IHandModuleInputReferenceConfig));
            }
            
            builder
                .RegisterEntryPoint<RightHandActionBasedTrackingModule>(Lifetime.Singleton)
                .As<IRightHandTrackingModule>();

            builder
                .RegisterEntryPoint<LeftHandActionBasedTrackingModule>(Lifetime.Singleton)
                .As<ILeftHandTrackingModule>();
        }
    }
}