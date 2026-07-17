using System.Threading;
using Cysharp.Threading.Tasks;
using Pet.Configs;
using UnityEngine;
using VContainer;

namespace Pet.UI
{
    public class UILoadingOverlay : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        private int transitionVersion;

        public void SetVisible(bool visible)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.blocksRaycasts = visible;
            canvasGroup.interactable = visible;
        }

        public async UniTask ShowAsync(float duration, CancellationToken cancellation)
        {
            await FadeToAsync(duration, 1f, cancellation);
            await UniTask.NextFrame(cancellation);
        }

        public UniTask HideAsync(float duration, CancellationToken cancellation)
        {
            return FadeToAsync(duration, 0, cancellation);
        }

        private async UniTask FadeToAsync(float duration, float targetAlpha, CancellationToken cancellation)
        {
            int version = ++transitionVersion;

            float startAlpha = canvasGroup.alpha;
            if (Mathf.Approximately(startAlpha, targetAlpha) || duration <= 0f)
            {
                canvasGroup.alpha = targetAlpha;
                return;
            }
            
            

            /*float elapsed = 0f;
            while (elapsed < duration)
            {
                cancellation.ThrowIfCancellationRequested();

                if (version != transitionVersion)
                {
                    return;
                }

                elapsed += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);

                await UniTask.Yield(PlayerLoopTiming.Update, cancellation);
            }

            if (version != transitionVersion)
            {
                return;
            }*/

            canvasGroup.alpha = targetAlpha;
        }
    }
}
