using Pet.Configs;
using Pet.Input;
using Pet.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Pet
{
    public class GlobalScope : LifetimeScope
    {
        [SerializeField] private ProjectConfig projectConfig;
        [SerializeField] private UiRoot uiRoot;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(projectConfig);
            builder.RegisterComponent(uiRoot);
            builder.RegisterComponent(uiRoot.LoadingOverlay);
            builder.Register<LoadingOverlayController>(Lifetime.Singleton);
            builder.Register<SceneLoader>(Lifetime.Singleton);
            builder.Register<InputActionsProvider>(Lifetime.Singleton);
            builder.Register<IPlayerInputStreams, PlayerInputStreams>(Lifetime.Singleton);

            builder.RegisterEntryPoint<Bootstrap>();
        }
    }
}
