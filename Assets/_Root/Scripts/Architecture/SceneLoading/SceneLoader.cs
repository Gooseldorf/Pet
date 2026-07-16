using System.Threading;
using Architecture.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Architecture.SceneLoading
{
    public class SceneLoader
    {
        private readonly LoadingOverlayController loadingOverlayController;
        private readonly LifetimeScope lifetimeScope;

        public SceneLoader(LoadingOverlayController loadingOverlayController, LifetimeScope lifetimeScope)
        {
            this.loadingOverlayController = loadingOverlayController;
            this.lifetimeScope = lifetimeScope;
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
                using (LifetimeScope.EnqueueParent(lifetimeScope))
                {
                    await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                }
            }
            finally
            {
                await loadingOverlayController.HideAsync(cancellation);
            }
        }
    }
}
