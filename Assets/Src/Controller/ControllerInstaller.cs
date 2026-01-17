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
        [ReadOnly]
        private IRightHandTrackingModule _rightHandTrackingModule;
        
        [OdinSerialize]
        [Inject]
        [ReadOnly]
        private ILeftHandTrackingModule _leftHandTrackingModule;

        public void Install(IContainerBuilder builder) {

            if (_handModuleInputReferenceConfig is not null) {
                builder
                    .RegisterInstance(_handModuleInputReferenceConfig)
                    .As(typeof(IHandModuleInputReferenceConfig));
            }
            
            builder
                .RegisterEntryPoint<RightHandTrackingModule>(Lifetime.Singleton)
                .As<IRightHandTrackingModule>();

            builder
                .RegisterEntryPoint<LeftHandTrackingModule>(Lifetime.Singleton)
                .As<ILeftHandTrackingModule>();
        }
    }
}