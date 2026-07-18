using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pet.UI;

namespace Pet.Gameplay
{
    public class UIGameplayController : IBackHandler, IDisposable
    {
        private readonly UIPauseMenuController pauseMenuController;
        private readonly UIBackRouter uiBackRouter;

        private IDisposable backRegistration;

        public UIGameplayController(UIPauseMenuController pauseMenuController, UIBackRouter uiBackRouter)
        {
            this.pauseMenuController = pauseMenuController;
            this.uiBackRouter = uiBackRouter;
        }

        public void Initialize()
        {
            backRegistration ??= uiBackRouter.Register(this);
        }

        public UniTask<bool> TryHandleBackAsync(CancellationToken cancellation)
        {
            return pauseMenuController.OpenAsync(cancellation);
        }

        public void Dispose()
        {
            backRegistration?.Dispose();
            backRegistration = null;
        }
    }
}
