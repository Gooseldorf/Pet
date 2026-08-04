using System.Threading;
using Cysharp.Threading.Tasks;
using Pet.Input;
using Pet;
using Unity.Cinemachine;
using UnityEngine;

namespace Pet.Gameplay
{
    public class GameplayEntryPoint : ISceneEntryPoint
    {
        private readonly InputActionsProvider inputActionsProvider;
        private readonly SpiderPlayerSpawner spiderPlayerSpawner;
        private readonly CameraSpawner cameraSpawner;
        private readonly Camera gameplayCamera;
        private readonly UIHudController gameplayHudController;
        private readonly UIGameplayController uiGameplayController;

        public GameplayEntryPoint(
            InputActionsProvider inputActionsProvider,
            SpiderPlayerSpawner spiderPlayerSpawner,
            CameraSpawner cameraSpawner,
            Camera gameplayCamera,
            UIHudController gameplayHudController,
            UIGameplayController uiGameplayController)
        {
            this.inputActionsProvider = inputActionsProvider;
            this.spiderPlayerSpawner = spiderPlayerSpawner;
            this.cameraSpawner = cameraSpawner;
            this.gameplayCamera = gameplayCamera;
            this.gameplayHudController = gameplayHudController;
            this.uiGameplayController = uiGameplayController;
        }

        public async UniTask InitializeAsync(CancellationToken cancellation)
        {
            SpiderPlayerController player = spiderPlayerSpawner.Spawn();
            CameraRig cameraRig = cameraSpawner.Spawn();
            CinemachineBrain brain = gameplayCamera.GetComponent<CinemachineBrain>();
            cameraRig.Bind(player, brain);
            inputActionsProvider.SetEnabledMaps(InputMapKind.PlayerAndUI);
            uiGameplayController.Initialize();
            await gameplayHudController.ShowAsync(cancellation);
        }
    }
}
