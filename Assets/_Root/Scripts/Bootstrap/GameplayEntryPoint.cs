using System.Threading;
using Cysharp.Threading.Tasks;
using Pet.Input;
using VContainer.Unity;

namespace Pet.Gameplay
{
    public class GameplayEntryPoint : IAsyncStartable
    {
        private readonly InputActionsProvider inputActionsProvider;
        private readonly UIHudController gameplayHudController;
        private readonly UIGameplayController uiGameplayController;

        public GameplayEntryPoint(
            InputActionsProvider inputActionsProvider,
            UIHudController gameplayHudController,
            UIGameplayController uiGameplayController)
        {
            this.inputActionsProvider = inputActionsProvider;
            this.gameplayHudController = gameplayHudController;
            this.uiGameplayController = uiGameplayController;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            inputActionsProvider.SetEnabledMaps(InputMapKind.PlayerAndUI);
            uiGameplayController.Initialize();
            await gameplayHudController.ShowAsync(cancellation);
        }
    }
}
