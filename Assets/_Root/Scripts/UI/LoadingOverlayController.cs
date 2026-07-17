using System.Threading;
using Cysharp.Threading.Tasks;

namespace Pet.UI
{
    public class LoadingOverlayController
    {
        private readonly LoadingOverlay loadingOverlay;

        public LoadingOverlayController(LoadingOverlay loadingOverlay)
        {
            this.loadingOverlay = loadingOverlay;
        }

        public UniTask ShowAsync(CancellationToken cancellation)
        {
            return loadingOverlay.ShowAsync(cancellation);
        }

        public UniTask HideAsync(CancellationToken cancellation)
        {
            return loadingOverlay.HideAsync(cancellation);
        }
    }
}
