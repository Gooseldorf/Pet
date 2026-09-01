using System.Threading;
using Cysharp.Threading.Tasks;
using Pet.Input;
using Pet;
using UnityEngine;

namespace Pet.Gameplay
{
    public class GameplayEntryPoint : ISceneEntryPoint
    {
        private readonly InputActionsProvider inputActionsProvider;
        private readonly SpiderPlayerSpawner spiderPlayerSpawner;
        private readonly CameraSpawner cameraSpawner;
        private readonly GameplayTester gameplayTester;
        private readonly UIHudController gameplayHudController;
        private readonly UIGameplayController uiGameplayController;

        public GameplayEntryPoint(
            InputActionsProvider inputActionsProvider,
            SpiderPlayerSpawner spiderPlayerSpawner,
            CameraSpawner cameraSpawner,
            GameplayTester gameplayTester,
            UIHudController gameplayHudController,
            UIGameplayController uiGameplayController)
        {
            this.inputActionsProvider = inputActionsProvider;
            this.spiderPlayerSpawner = spiderPlayerSpawner;
            this.cameraSpawner = cameraSpawner;
            this.gameplayTester = gameplayTester;
            this.gameplayHudController = gameplayHudController;
            this.uiGameplayController = uiGameplayController;
        }

        public async UniTask InitializeAsync(CancellationToken cancellation)
        {
            SpiderPlayerController player = spiderPlayerSpawner.Spawn();
            CameraRig cameraRig = cameraSpawner.Spawn();
            cameraRig.Bind(player);
            gameplayTester.Initialize(player, cameraRig);
            inputActionsProvider.SetEnabledMaps(InputMapKind.PlayerAndUI);
            uiGameplayController.Initialize();
            await gameplayHudController.ShowAsync(cancellation);
            
            Debug.Log("Gameplay Initialized");
        }
    }
}
