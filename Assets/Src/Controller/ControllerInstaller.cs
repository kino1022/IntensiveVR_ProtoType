using Sirenix.OdinInspector;
using Sirenix.Serialization;
using VContainer;
using VContainer.Unity;

namespace Controller {
    public class ControllerInstaller : SerializedMonoBehaviour, IInstaller {

        [OdinSerialize]
        [LabelText("右手トラッキングモジュール")]
        private IRightHandTrackingModule _rightModule;
        
        [OdinSerialize]
        [LabelText("左手トラッキングモジュール")]
        private ILeftHandTrackingModule _leftModule;
        
        public void Install(IContainerBuilder builder) {

            if (_rightModule is not null) {
                builder
                    .RegisterInstance(_rightModule)
                    .As<IRightHandTrackingModule>();
            }

            if (_leftModule is not null) {
                builder
                    .RegisterInstance(_leftModule)
                    .As<ILeftHandTrackingModule>();
            }

            builder
                .RegisterEntryPoint<LeftTrackingPositionProvider>()
                .As<ILeftTrackingPositionProvider>();
            
            builder
                .RegisterEntryPoint<RightTrackingPositionProvider>()
                .As<IRightTrackingPositionProvider>();
            
        }
    }
}