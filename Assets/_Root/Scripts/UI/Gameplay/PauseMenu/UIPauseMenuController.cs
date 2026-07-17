using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pet.UI;

namespace Pet.Gameplay
{
    public class UIPauseMenuController : IDisposable
    {
        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private readonly UIPopupCoordinator popupCoordinator;
        private readonly UIPauseMenuConfig pausePopupConfig;
        private readonly SceneLoader sceneLoader;

        public UIPauseMenuController(UIPopupCoordinator popupCoordinator, UIPauseMenuConfig pausePopupConfig, SceneLoader sceneLoader)
        {
            this.popupCoordinator = popupCoordinator;
            this.pausePopupConfig = pausePopupConfig;
            this.sceneLoader = sceneLoader;
        }

        public async UniTask<bool> OpenAsync(CancellationToken cancellation)
        {
            UIPopupHandle popupHandle = await popupCoordinator.ShowAsync(pausePopupConfig, cancellation);
            if (popupHandle?.View is not UIPausePopupView view)
            {
                return popupHandle != null;
            }

            view.SetCallbacks(HandleResumeRequested, HandleReturnToMenuRequested);
            return true;
        }

        public void Dispose()
        {
            disposeCancellationTokenSource.Cancel();
            disposeCancellationTokenSource.Dispose();
        }

        private void HandleResumeRequested()
        {
            ResumeAsync().Forget();
        }

        private void HandleReturnToMenuRequested()
        {
            ReturnToMenuAsync().Forget();
        }

        private async UniTaskVoid ResumeAsync()
        {
            await popupCoordinator.CloseCurrentAsync(disposeCancellationTokenSource.Token);
        }

        private async UniTaskVoid ReturnToMenuAsync()
        {
            CancellationToken cancellation = disposeCancellationTokenSource.Token;
            await popupCoordinator.CloseCurrentAsync(cancellation);
            await sceneLoader.SwitchToSceneAsync("MainMenu", cancellation);
        }
    }
}
