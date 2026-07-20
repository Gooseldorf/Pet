using System.Threading;
using Cysharp.Threading.Tasks;
using Pet.Input;
using Pet;

namespace Pet.MainMenu
{
    public class MainMenuEntryPoint : ISceneEntryPoint
    {
        private readonly InputActionsProvider inputActionsProvider;
        private readonly UIMainMenuController mainMenuController;

        public MainMenuEntryPoint(InputActionsProvider inputActionsProvider, UIMainMenuController mainMenuController)
        {
            this.inputActionsProvider = inputActionsProvider;
            this.mainMenuController = mainMenuController;
        }

        public UniTask InitializeAsync(CancellationToken cancellation)
        {
            inputActionsProvider.SetEnabledMaps(InputMapKind.UI);
            return mainMenuController.ShowAsync(cancellation);
        }
    }
}
