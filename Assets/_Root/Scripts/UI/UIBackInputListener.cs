using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pet.Input;
using R3;
using VContainer.Unity;

namespace Pet.UI
{
    public class UIBackInputListener : IStartable, IDisposable
    {
        private readonly CancellationTokenSource disposeCancellationTokenSource = new();
        private readonly List<IDisposable> subscriptions = new();

        private readonly InputActionsProvider inputActionsProvider;
        private readonly UIBackRouter uiBackRouter;

        private bool isHandlingBack;

        public UIBackInputListener(InputActionsProvider inputActionsProvider, UIBackRouter uiBackRouter)
        {
            this.inputActionsProvider = inputActionsProvider;
            this.uiBackRouter = uiBackRouter;
        }

        public void Start()
        {
            subscriptions.Add(inputActionsProvider.Cancel
                .PerformedAsObservable(disposeCancellationTokenSource.Token)
                .Subscribe(_ => HandleBackRequest().Forget()));
        }

        public void Dispose()
        {
            disposeCancellationTokenSource.Cancel();

            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            }

            disposeCancellationTokenSource.Dispose();
        }

        private async UniTaskVoid HandleBackRequest()
        {
            if (isHandlingBack)
            {
                return;
            }

            isHandlingBack = true;

            try
            {
                await uiBackRouter.HandleBackAsync(disposeCancellationTokenSource.Token);
            }
            finally
            {
                isHandlingBack = false;
            }
        }
    }
}
