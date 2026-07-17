using System.Threading;
using Cysharp.Threading.Tasks;
using Pet.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Pet
{
    public class SceneLoader
    {
        private const string BOOTSTRAP_SCENE_NAME = "Bootstrap";

        private readonly UILoadingOverlayController loadingOverlayController;
        private readonly LifetimeScope lifetimeScope;

        public SceneLoader(UILoadingOverlayController loadingOverlayController, LifetimeScope lifetimeScope)
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

        public async UniTask SwitchToSceneAsync(string sceneName, CancellationToken cancellation = default)
        {
            Scene targetScene = SceneManager.GetSceneByName(sceneName);
            bool targetAlreadyLoaded = targetScene.isLoaded;
            bool hasOtherLoadedContentScenes = HasLoadedContentSceneExcept(sceneName);

            if (targetAlreadyLoaded && !hasOtherLoadedContentScenes)
            {
                SceneManager.SetActiveScene(targetScene);
                return;
            }

            await loadingOverlayController.ShowAsync(cancellation);

            try
            {
                if (!targetAlreadyLoaded)
                {
                    using (LifetimeScope.EnqueueParent(lifetimeScope))
                    {
                        await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    }

                    targetScene = SceneManager.GetSceneByName(sceneName);
                }

                await UnloadOtherContentScenesAsync(sceneName);
                SceneManager.SetActiveScene(targetScene);
            }
            finally
            {
                await loadingOverlayController.HideAsync(cancellation);
            }
        }

        private static bool HasLoadedContentSceneExcept(string sceneName)
        {
            for (int index = 0; index < SceneManager.sceneCount; index++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(index);
                if (!loadedScene.isLoaded || loadedScene.name == BOOTSTRAP_SCENE_NAME || loadedScene.name == sceneName)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private static async UniTask UnloadOtherContentScenesAsync(string targetSceneName)
        {
            for (int index = SceneManager.sceneCount - 1; index >= 0; index--)
            {
                Scene loadedScene = SceneManager.GetSceneAt(index);
                if (!loadedScene.isLoaded || loadedScene.name == BOOTSTRAP_SCENE_NAME || loadedScene.name == targetSceneName)
                {
                    continue;
                }

                await SceneManager.UnloadSceneAsync(loadedScene);
            }
        }
    }
}
