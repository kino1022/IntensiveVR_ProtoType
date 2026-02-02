using VContainer;
using VContainer.Unity;

namespace Player.Scope {
    public class PlayerLifetimeScope : LifetimeScope {
        protected override void Configure(IContainerBuilder builder) {
            base.Configure(builder);

            var speedManager = gameObject.transform.root.GetComponentInChildren<IWalkSpeedManager>();

            if (speedManager is not null) {
                builder
                    .RegisterComponent(speedManager)
                    .As<IWalkSpeedManager>()
                    .As<IWalkSpeedProvider>();
            }
        }
    }
}