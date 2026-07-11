using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Architecture.UI
{
    public class LoadingOverlay : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.2f;

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

                elapsed += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(elapsed / fadeDuration);
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);

                await UniTask.Yield(PlayerLoopTiming.Update, cancellation);
            }

            canvasGroup.alpha = targetAlpha;
        }
    }
}
