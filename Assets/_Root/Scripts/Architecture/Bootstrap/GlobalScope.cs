using Architecture.Bootstrap;
using Architecture.SceneLoading;
using Architecture.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GlobalScope : LifetimeScope
{
    [SerializeField] private UiRoot uiRoot;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(uiRoot);
        builder.RegisterInstance(uiRoot.LoadingOverlay);
        builder.Register<LoadingOverlayController>(Lifetime.Singleton);
        builder.Register<SceneLoader>(Lifetime.Singleton);
        
        builder.RegisterEntryPoint<Bootstrap>();
    }
}
