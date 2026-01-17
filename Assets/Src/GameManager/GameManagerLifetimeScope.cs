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
        private ControllerInstaller _controllerInstaller;
        
        protected override void Configure(IContainerBuilder builder) {
            if (_controllerInstaller != null) {
                _controllerInstaller.Install(builder);
            }
        }
    }
}