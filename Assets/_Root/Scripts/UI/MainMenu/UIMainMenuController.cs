using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pet.UI;

namespace Pet.MainMenu
{
    public class UIMainMenuController : IDisposable
    {
        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private readonly SceneLoader sceneLoader;
        private readonly UIMainMenuConfig mainMenuScreenConfig;
        private readonly UIScreenNavigator screenNavigator;

        public UIMainMenuController(SceneLoader sceneLoader, UIMainMenuConfig mainMenuScreenConfig, UIScreenNavigator screenNavigator)
        {
            this.sceneLoader = sceneLoader;
            this.mainMenuScreenConfig = mainMenuScreenConfig;
            this.screenNavigator = screenNavigator;
        }

        public async UniTask ShowAsync(CancellationToken cancellation)
        {
            UIScreenHandle screenHandle = await screenNavigator.OpenAsync(mainMenuScreenConfig, cancellation);
            UIMainMenuScreenView view = (UIMainMenuScreenView)screenHandle.View;
            view.SetCallbacks(HandlePlayRequested);
        }

        public void Dispose()
        {
            disposeCancellationTokenSource.Cancel();
            disposeCancellationTokenSource.Dispose();
        }

        private void HandlePlayRequested()
        {
            StartGameplayAsync().Forget();
        }

        private async UniTaskVoid StartGameplayAsync()
        {
            CancellationToken cancellation = disposeCancellationTokenSource.Token;
            await screenNavigator.ClearAsync(cancellation);
            await sceneLoader.SwitchToSceneAsync("Gameplay", cancellation);
        }
    }
}
