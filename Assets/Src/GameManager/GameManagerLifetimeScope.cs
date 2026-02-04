using Cam;
using Controller;
using Controller.Hand;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameManager {
    public class GameManagerLifetimeScope : LifetimeScope {
        
        [SerializeField]
        private Camera _mainCamera;

        [SerializeField]
        private Camera _xrOriginCamara;
        
        [SerializeField]
        private ControllerInstaller _controllerInstaller;
        
        protected override void Configure(IContainerBuilder builder) {
            if (_controllerInstaller != null) {
                _controllerInstaller.Install(builder);
            }

            if (_mainCamera is not null) {
                var cam = new MainIdentifiedCamera(_mainCamera);
                builder
                    .RegisterInstance(cam)
                    .As<IMainIdentifiedCamera>();
            }

            if (_xrOriginCamara is not null) {
                var cam = new XROriginIdentifiedCamera(_xrOriginCamara);
                builder
                    .RegisterInstance(cam)
                    .As<IXROriginIdentifiedCamera>();
            }
        }

        private void RegisterIdentifiedCamera<TInterface, TType>(IContainerBuilder builder, Camera cam)
            where TInterface : ICameraIdentify where TType : CameraIdentify {
            var idcam = System.Activator.CreateInstance(typeof(TType), cam);
            if (idcam is null) {
                Debug.Log($"{GetType().Name}: Failed to create instance of {typeof(TType).Name}");
            }

            builder
                .RegisterInstance(idcam)
                .AsSelf();
        }
    }
}