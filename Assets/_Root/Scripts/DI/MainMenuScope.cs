using Pet;
using Pet.UI;
using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace Pet.MainMenu
{
    public class MainMenuScope : LifetimeScope
    {
        [SerializeField] private UIMainMenuConfig mainMenuScreenConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            //Configs:
            builder.RegisterInstance(mainMenuScreenConfig);
            
            //Controllers:
            builder.Register<UIMainMenuController>(Lifetime.Scoped);

            //Scene startup:
            builder.Register<ISceneEntryPoint, MainMenuEntryPoint>(Lifetime.Scoped);
        }
    }
}
