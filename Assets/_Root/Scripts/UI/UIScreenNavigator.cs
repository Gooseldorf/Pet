using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Pet.UI
{
    public class UIScreenNavigator
    {
        private readonly UIInstanceFactory uiInstanceFactory;
        private readonly Stack<UIScreenHandle> history = new();

        private UIScreenHandle currentScreen;

        public UIScreenNavigator(UIInstanceFactory uiInstanceFactory)
        {
            this.uiInstanceFactory = uiInstanceFactory;
        }

        public bool CanGoBack => history.Count > 0;
        public UIScreenHandle CurrentScreen => currentScreen;

        public async UniTask<UIScreenHandle> OpenAsync(UIScreenConfigBase config, CancellationToken cancellation = default)
        {
            if (currentScreen != null && currentScreen.Config == config)
            {
                await currentScreen.View.ShowAsync(cancellation);
                return currentScreen;
            }

            switch (config.HistoryModeEnum)
            {
                case UIHistoryModeEnum.ClearAndOpen:
                    await ClearAsync(cancellation);
                    break;
                case UIHistoryModeEnum.Push:
                    if (currentScreen != null)
                    {
                        await currentScreen.View.HideAsync(cancellation);
                        history.Push(currentScreen);
                        currentScreen = null;
                    }

                    break;
                case UIHistoryModeEnum.Replace:
                case UIHistoryModeEnum.None:
                    if (currentScreen != null)
                    {
                        await CloseCurrentAsync(cancellation);
                    }

                    break;
            }

            currentScreen = uiInstanceFactory.GetScreen(config);
            await currentScreen.View.ShowAsync(cancellation);
            return currentScreen;
        }

        public async UniTask<bool> BackAsync(CancellationToken cancellation = default)
        {
            if (currentScreen == null || history.Count == 0)
            {
                return false;
            }

            await CloseCurrentAsync(cancellation);
            currentScreen = history.Pop();
            await currentScreen.View.ShowAsync(cancellation);
            return true;
        }

        public void Clear()
        {
            if (currentScreen != null)
            {
                uiInstanceFactory.Release(currentScreen);
            }

            while (history.Count > 0)
            {
                UIScreenHandle hiddenScreen = history.Pop();
                uiInstanceFactory.Release(hiddenScreen);
            }
        }

        public async UniTask ClearAsync(CancellationToken cancellation = default)
        {
            if (currentScreen != null)
            {
                await CloseCurrentAsync(cancellation);
            }

            while (history.Count > 0)
            {
                UIScreenHandle hiddenScreen = history.Pop();
                uiInstanceFactory.Release(hiddenScreen);
            }
        }

        private async UniTask CloseCurrentAsync(CancellationToken cancellation)
        {
            await currentScreen.View.HideAsync(cancellation);
            uiInstanceFactory.Release(currentScreen);
            currentScreen = null;
        }
    }
}
