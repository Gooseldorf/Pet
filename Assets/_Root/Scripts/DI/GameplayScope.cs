using Pet;
using Pet.UI;
using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace Pet.Gameplay
{
    public class GameplayScope : LifetimeScope
    {
        [SerializeField] private SpiderConfig spiderConfig;
        [SerializeField] private PlayerSpawnPoint spiderSpawnPoint;
        [SerializeField] private UIHudConfig gameplayHudConfig;
        [SerializeField] private UIPauseMenuConfig pauseMenuConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(spiderConfig);
            builder.RegisterComponent(spiderSpawnPoint);
            builder.Register<SpiderPlayerSpawner>(Lifetime.Scoped);
            builder.RegisterInstance(gameplayHudConfig);
            builder.RegisterInstance(pauseMenuConfig);
            builder.Register<UIHudController>(Lifetime.Scoped);
            builder.Register<UIPauseMenuController>(Lifetime.Scoped);
            builder.Register<UIGameplayController>(Lifetime.Scoped);
            builder.Register<ISceneEntryPoint, GameplayEntryPoint>(Lifetime.Scoped);
        }
    }
}
