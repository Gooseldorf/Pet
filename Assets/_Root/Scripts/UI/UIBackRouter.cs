using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Pet.UI
{
    public class UIBackRouter
    {
        private readonly UIPopupCoordinator popupCoordinator;
        private readonly UIScreenNavigator screenNavigator;
        private readonly List<IBackHandler> backHandlers = new();

        public UIBackRouter(UIPopupCoordinator popupCoordinator, UIScreenNavigator screenNavigator)
        {
            this.popupCoordinator = popupCoordinator;
            this.screenNavigator = screenNavigator;
        }

        public IDisposable Register(IBackHandler backHandler)
        {
            backHandlers.Add(backHandler);
            return new BackHandlerRegistration(this, backHandler);
        }

        public async UniTask<bool> HandleBackAsync(CancellationToken cancellation = default)
        {
            if (popupCoordinator.HasOpenPopup)
            {
                return await popupCoordinator.CloseCurrentAsync(cancellation);
            }

            if (screenNavigator.CanGoBack)
            {
                return await screenNavigator.BackAsync(cancellation);
            }

            for (int index = backHandlers.Count - 1; index >= 0; index--)
            {
                if (await backHandlers[index].TryHandleBackAsync(cancellation))
                {
                    return true;
                }
            }

            return false;
        }

        private void Unregister(IBackHandler backHandler)
        {
            backHandlers.Remove(backHandler);
        }

        private sealed class BackHandlerRegistration : IDisposable
        {
            private readonly UIBackRouter owner;
            private IBackHandler backHandler;

            public BackHandlerRegistration(UIBackRouter owner, IBackHandler backHandler)
            {
                this.owner = owner;
                this.backHandler = backHandler;
            }

            public void Dispose()
            {
                if (backHandler == null)
                {
                    return;
                }

                owner.Unregister(backHandler);
                backHandler = null;
            }
        }
    }
}
