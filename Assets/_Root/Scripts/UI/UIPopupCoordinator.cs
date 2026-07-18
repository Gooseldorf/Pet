using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Pet.UI
{
    public class UIPopupCoordinator
    {
        private readonly UIInstanceFactory uiInstanceFactory;
        private readonly Queue<UIPopupConfigBase> queuedPopups = new();

        private UIPopupHandle currentPopup;

        public UIPopupCoordinator(UIInstanceFactory uiInstanceFactory)
        {
            this.uiInstanceFactory = uiInstanceFactory;
        }

        public bool HasOpenPopup => currentPopup != null;
        public UIPopupHandle CurrentPopup => currentPopup;

        public async UniTask<UIPopupHandle> ShowAsync(UIPopupConfigBase config, CancellationToken cancellation = default)
        {
            if (currentPopup == null)
            {
                return await ShowNowAsync(config, cancellation);
            }

            switch (config.QueueModeEnum)
            {
                case UiPopupQueueModeEnum.ReplaceCurrent:
                    await CloseCurrentAsync(cancellation);
                    return await ShowNowAsync(config, cancellation);
                case UiPopupQueueModeEnum.DropIfBusy:
                    return null;
                case UiPopupQueueModeEnum.Enqueue:
                case UiPopupQueueModeEnum.ShowImmediatelyIfFree:
                    queuedPopups.Enqueue(config);
                    return null;
                default:
                    return null;
            }
        }

        public async UniTask<bool> CloseCurrentAsync(CancellationToken cancellation = default)
        {
            if (currentPopup == null)
            {
                return false;
            }

            UIPopupHandle popupToClose = currentPopup;
            currentPopup = null;

            await popupToClose.View.HideAsync(cancellation);
            uiInstanceFactory.Release(popupToClose);

            if (queuedPopups.Count > 0)
            {
                UIPopupConfigBase nextPopup = queuedPopups.Dequeue();
                await ShowNowAsync(nextPopup, cancellation);
            }

            return true;
        }

        private async UniTask<UIPopupHandle> ShowNowAsync(UIPopupConfigBase config, CancellationToken cancellation)
        {
            currentPopup = uiInstanceFactory.GetPopup(config);
            await currentPopup.View.ShowAsync(cancellation);
            return currentPopup;
        }
    }
}
