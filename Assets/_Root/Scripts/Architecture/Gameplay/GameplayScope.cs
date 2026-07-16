using VContainer;
using VContainer.Unity;

namespace Architecture.Gameplay
{
    public class GameplayScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameplayEntryPoint>();
        }
    }
}
