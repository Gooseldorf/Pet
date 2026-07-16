using VContainer;
using VContainer.Unity;

namespace Architecture.MainMenu
{
    public class MainMenuScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MainMenuEntryPoint>();
        }
    }
}
