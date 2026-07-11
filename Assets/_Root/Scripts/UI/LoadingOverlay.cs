using System.Threading;
using Architecture.Configs;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Architecture.UI
{
    public class LoadingOverlay : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        private ProjectConfig projectConfig;
        private int transitionVersion;

        [Inject]
        public void Construct(ProjectConfig projectConfig)
        {
            this.projectConfig = projectConfig;
        }

        public void SetVisible(bool visible)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.blocksRaycasts = visible;
            canvasGroup.interactable = visible;
        }

        public UniTask ShowAsync(CancellationToken cancellation)
        {
            return FadeToAsync(1f, true, cancellation);
        }

        public UniTask HideAsync(CancellationToken cancellation)
        {
            return FadeToAsync(0f, false, cancellation);
        }

        private async UniTask FadeToAsync(float targetAlpha, bool interactive, CancellationToken cancellation)
        {
            int version = ++transitionVersion;
            float fadeDuration = projectConfig.UI.LoadingOverlay.FadeDuration;

            canvasGroup.blocksRaycasts = interactive;
            canvasGroup.interactable = interactive;

            float startAlpha = canvasGroup.alpha;
            if (Mathf.Approximately(startAlpha, targetAlpha))
            {
                canvasGroup.alpha = targetAlpha;
                return;
            }

            if (fadeDuration <= 0f)
            {
                canvasGroup.alpha = targetAlpha;
                return;
            }

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                cancellation.ThrowIfCancellationRequested();

                if (version != transitionVersion)
                {
                    return;
                }

                elapsed += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(elapsed / fadeDuration);
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);

                await UniTask.Yield(PlayerLoopTiming.Update, cancellation);
            }

            if (version != transitionVersion)
            {
                return;
            }

            canvasGroup.alpha = targetAlpha;
        }
    }
}
