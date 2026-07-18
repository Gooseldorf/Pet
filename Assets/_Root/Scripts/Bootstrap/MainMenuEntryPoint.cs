using System.Threading;
using Cysharp.Threading.Tasks;
using Pet.Input;
using VContainer.Unity;

namespace Pet.MainMenu
{
    public class MainMenuEntryPoint : IAsyncStartable
    {
        private readonly InputActionsProvider inputActionsProvider;
        private readonly UIMainMenuController mainMenuController;

        public MainMenuEntryPoint(InputActionsProvider inputActionsProvider, UIMainMenuController mainMenuController)
        {
            this.inputActionsProvider = inputActionsProvider;
            this.mainMenuController = mainMenuController;
        }

        public UniTask StartAsync(CancellationToken cancellation)
        {
            inputActionsProvider.SetEnabledMaps(InputMapKind.UI);
            return mainMenuController.ShowAsync(cancellation);
        }
    }
}
