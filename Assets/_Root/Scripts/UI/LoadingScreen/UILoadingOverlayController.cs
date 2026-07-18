using System.Threading;
using Cysharp.Threading.Tasks;
using Pet.Configs;

namespace Pet.UI
{
    public class UILoadingOverlayController
    {
        private readonly UILoadingOverlay view;
        private readonly UILoadingOverlayConfig config;

        public UILoadingOverlayController(UILoadingOverlayConfig config, UILoadingOverlay view)
        {
            this.config = config;
            this.view = view;
        }

        public UniTask ShowAsync(CancellationToken cancellation)
        {
            return view.ShowAsync(config.FadeDuration, cancellation);
        }

        public UniTask HideAsync(CancellationToken cancellation)
        {
            return view.HideAsync(config.FadeDuration, cancellation);
        }
    }
}
