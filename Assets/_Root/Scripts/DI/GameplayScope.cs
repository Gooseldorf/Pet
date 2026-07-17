using Pet.UI;
using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace Pet.Gameplay
{
    public class GameplayScope : LifetimeScope
    {
        [SerializeField] private UIHudConfig gameplayHudConfig;
        [SerializeField] private UIPauseMenuConfig pauseMenuConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(gameplayHudConfig);
            builder.RegisterInstance(pauseMenuConfig);
            builder.Register<UIHudController>(Lifetime.Scoped);
            builder.Register<UIPauseMenuController>(Lifetime.Scoped);
            builder.Register<UIGameplayController>(Lifetime.Scoped);
            builder.RegisterEntryPoint<GameplayEntryPoint>();
        }
    }
}
