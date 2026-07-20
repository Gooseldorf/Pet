using System.Threading;
using Cysharp.Threading.Tasks;
using Pet.Input;
using Pet;

namespace Pet.Gameplay
{
    public class GameplayEntryPoint : ISceneEntryPoint
    {
        private readonly InputActionsProvider inputActionsProvider;
        private readonly SpiderPlayerSpawner spiderPlayerSpawner;
        private readonly UIHudController gameplayHudController;
        private readonly UIGameplayController uiGameplayController;

        public GameplayEntryPoint(
            InputActionsProvider inputActionsProvider,
            SpiderPlayerSpawner spiderPlayerSpawner,
            UIHudController gameplayHudController,
            UIGameplayController uiGameplayController)
        {
            this.inputActionsProvider = inputActionsProvider;
            this.spiderPlayerSpawner = spiderPlayerSpawner;
            this.gameplayHudController = gameplayHudController;
            this.uiGameplayController = uiGameplayController;
        }

        public async UniTask InitializeAsync(CancellationToken cancellation)
        {
            spiderPlayerSpawner.Spawn();
            inputActionsProvider.SetEnabledMaps(InputMapKind.PlayerAndUI);
            uiGameplayController.Initialize();
            await gameplayHudController.ShowAsync(cancellation);
        }
    }
}
