using VContainer;
using VContainer.Unity;

namespace Pet.Gameplay
{
    public class GameplayScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameplayEntryPoint>();
        }
    }
}
