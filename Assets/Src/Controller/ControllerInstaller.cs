using Controller.Hand;
using Sirenix.OdinInspector;
using VContainer;
using VContainer.Unity;

namespace Controller {
    public class ControllerInstaller : SerializedMonoBehaviour, IInstaller {

        public void Install(IContainerBuilder builder) {
            
            builder
                .RegisterEntryPoint<RightHandTrackingModule>(Lifetime.Singleton)
                .As<IRightHandTrackingModule>();

            builder
                .RegisterEntryPoint<LeftHandTrackingModule>(Lifetime.Singleton)
                .As<ILeftHandTrackingModule>();
        }
    }
}