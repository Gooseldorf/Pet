using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pet.UI;

namespace Pet.Gameplay
{
    public class UIHudController : IDisposable
    {
        private readonly UIHudConfig gameplayHudConfig;
        private readonly UIInstanceFactory uiInstanceFactory;

        private UIHudView hudView;

        public UIHudController(UIHudConfig gameplayHudConfig, UIInstanceFactory uiInstanceFactory)
        {
            this.gameplayHudConfig = gameplayHudConfig;
            this.uiInstanceFactory = uiInstanceFactory;
        }

        public UniTask ShowAsync(CancellationToken cancellation)
        {
            hudView = uiInstanceFactory.GetHud(gameplayHudConfig);
            return hudView.ShowAsync(cancellation);
        }

        public void Dispose()
        {
            if (hudView != null)
            {
                uiInstanceFactory.Release(gameplayHudConfig, hudView);
                hudView = null;
            }
        }
    }
}
