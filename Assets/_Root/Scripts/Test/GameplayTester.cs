using System;
using System.Threading;
using Pet.Input;
using R3;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Pet.Gameplay
{
    public class GameplayTester : IDisposable
    {
        private readonly CancellationTokenSource disposeCancellationTokenSource = new();
        private readonly InputActionsProvider inputActionsProvider;
        private readonly SpiderPlayerSpawner spiderPlayerSpawner;

        private IDisposable respawnSubscription;
        private SpiderPlayerController player;
        private CameraRig cameraRig;

        public GameplayTester(InputActionsProvider inputActionsProvider, SpiderPlayerSpawner spiderPlayerSpawner)
        {
            this.inputActionsProvider = inputActionsProvider;
            this.spiderPlayerSpawner = spiderPlayerSpawner;
        }

        public void Initialize(SpiderPlayerController player, CameraRig cameraRig)
        {
            this.player = player;
            this.cameraRig = cameraRig;
            respawnSubscription = inputActionsProvider.Respawn
                .PerformedAsObservable(disposeCancellationTokenSource.Token)
                .Subscribe(_ => Respawn());
        }

        public void Dispose()
        {
            disposeCancellationTokenSource.Cancel();
            respawnSubscription?.Dispose();
            disposeCancellationTokenSource.Dispose();
        }

        private void Respawn()
        {
            SpiderPlayerController previousPlayer = player;
            player = spiderPlayerSpawner.Spawn();
            cameraRig.Bind(player);
            Object.Destroy(previousPlayer.gameObject);
        }
    }
}
