using System.Threading;
using Cysharp.Threading.Tasks;
using Architecture.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Architecture.SceneLoading
{
    public class SceneLoader
    {
        private readonly LoadingOverlayController loadingOverlayController;

        public SceneLoader(LoadingOverlayController loadingOverlayController)
        {
            this.loadingOverlayController = loadingOverlayController;
        }

        public async UniTask LoadAdditiveAsync(string sceneName, CancellationToken cancellation = default)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene.isLoaded)
            {
                return;
            }

            await loadingOverlayController.ShowAsync(cancellation);

            try
            {
                await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
            finally
            {
                await loadingOverlayController.HideAsync(cancellation);
            }
        }
    }
}
