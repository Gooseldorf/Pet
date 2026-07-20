using Pet;
using Pet.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Pet.Gameplay
{
    public class GameplayScope : LifetimeScope
    {
        [SerializeField] private SpiderConfig spiderConfig;
        [SerializeField] private CameraConfig cameraConfig;
        [SerializeField] private PlayerSpawnPoint spiderSpawnPoint;
        [SerializeField] private UIHudConfig gameplayHudConfig;
        [SerializeField] private UIPauseMenuConfig pauseMenuConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(spiderConfig);
            builder.RegisterInstance(cameraConfig);
            builder.RegisterComponentInHierarchy<Camera>();
            builder.RegisterComponent(spiderSpawnPoint);
            builder.Register<SpiderPlayerSpawner>(Lifetime.Scoped);
            builder.Register<CameraSpawner>(Lifetime.Scoped);
            builder.RegisterInstance(gameplayHudConfig);
            builder.RegisterInstance(pauseMenuConfig);
            builder.Register<UIHudController>(Lifetime.Scoped);
            builder.Register<UIPauseMenuController>(Lifetime.Scoped);
            builder.Register<UIGameplayController>(Lifetime.Scoped);
            builder.Register<ISceneEntryPoint, GameplayEntryPoint>(Lifetime.Scoped);
        }
    }
}
