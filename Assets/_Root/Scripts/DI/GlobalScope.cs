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
        [SerializeField] private UILoadingOverlayConfig loadingOverlayConfig;
        [SerializeField] private UIRoot uiRoot;

        protected override void Configure(IContainerBuilder builder)
        {
            //Configs:
            builder.RegisterInstance(projectConfig);
            builder.RegisterInstance(loadingOverlayConfig);
            
            //Persistent components:
            builder.RegisterComponent(uiRoot);
            builder.RegisterComponent(uiRoot.LoadingOverlay);
            
            //Scene loading:
            builder.Register<SceneLoader>(Lifetime.Singleton);
            
            //UI:
            builder.Register<UILoadingOverlayController>(Lifetime.Singleton);
            builder.Register<UIInstanceFactory>(Lifetime.Singleton);
            builder.Register<UIScreenNavigator>(Lifetime.Singleton);
            builder.Register<UIPopupCoordinator>(Lifetime.Singleton);
            builder.Register<UIBackRouter>(Lifetime.Singleton);
            
            //Input:
            builder.Register<InputActionsProvider>(Lifetime.Singleton);
            builder.Register<IPlayerInputStreams, PlayerInputStreams>(Lifetime.Singleton);
            
            //Entry points:
            builder.RegisterEntryPoint<Bootstrap>();
            builder.RegisterEntryPoint<UIBackInputListener>();
        }
    }
}
