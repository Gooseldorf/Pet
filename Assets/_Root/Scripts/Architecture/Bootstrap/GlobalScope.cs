using Architecture.Bootstrap;
using Architecture.Configs;
using Architecture.Input;
using Architecture.Input.Player;
using Architecture.SceneLoading;
using Architecture.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

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
