using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pet.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
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

                scene = SceneManager.GetSceneByName(sceneName);
                SceneManager.SetActiveScene(scene);
                await InitializeSceneAsync(scene, cancellation);
            }
            finally
            {
                await loadingOverlayController.HideAsync(cancellation);
            }
        }

        public async UniTask SwitchToSceneAsync(string sceneName, CancellationToken cancellation = default)
        {
            await SwitchToSceneAsync(sceneName, null, cancellation);
        }

        public async UniTask SwitchToSceneAsync(string sceneName, Func<CancellationToken, UniTask> onLoadingOverlayShown, CancellationToken cancellation = default)
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
                if (onLoadingOverlayShown != null)
                {
                    await onLoadingOverlayShown(cancellation);
                }

                if (!targetAlreadyLoaded)
                {
                    using (LifetimeScope.EnqueueParent(lifetimeScope))
                    {
                        await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    }

                    targetScene = SceneManager.GetSceneByName(sceneName);
                }

                SceneManager.SetActiveScene(targetScene);

                if (!targetAlreadyLoaded)
                {
                    await InitializeSceneAsync(targetScene, cancellation);
                }

                await UnloadOtherContentScenesAsync(sceneName);
            }
            finally
            {
                await loadingOverlayController.HideAsync(cancellation);
            }
        }

        private static async UniTask InitializeSceneAsync(Scene scene, CancellationToken cancellation)
        {
            LifetimeScope sceneLifetimeScope = FindSceneLifetimeScope(scene);
            ISceneEntryPoint sceneEntryPoint = sceneLifetimeScope.Container.Resolve<ISceneEntryPoint>();
            await sceneEntryPoint.InitializeAsync(cancellation);
        }

        private static LifetimeScope FindSceneLifetimeScope(Scene scene)
        {
            GameObject[] rootObjects = scene.GetRootGameObjects();

            for (int i = 0; i < rootObjects.Length; i++)
            {
                if (rootObjects[i].TryGetComponent(out LifetimeScope sceneLifetimeScope))
                {
                    return sceneLifetimeScope;
                }
            }

            throw new InvalidOperationException($"Scene '{scene.name}' does not contain a root LifetimeScope.");
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
