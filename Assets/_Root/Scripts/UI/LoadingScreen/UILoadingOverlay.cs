using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Pet.Configs;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Pet.UI
{
    public class UILoadingOverlay : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        
        public void SetVisible(bool visible)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.blocksRaycasts = visible;
            canvasGroup.interactable = visible;
        }

        public async UniTask ShowAsync(float duration, CancellationToken cancellation)
        {
            gameObject.SetActive(true);
            await FadeToAsync(duration, 1f, cancellation);
            await UniTask.NextFrame(cancellation);
        }

        public async UniTask HideAsync(float duration, CancellationToken cancellation)
        {
            await FadeToAsync(duration, 0, cancellation);
            gameObject.SetActive(false);
        }

        private async UniTask FadeToAsync(float duration, float targetAlpha, CancellationToken cancellation)
        {
            if (Mathf.Approximately(canvasGroup.alpha, targetAlpha) || duration <= 0f)
            {
                canvasGroup.alpha = targetAlpha;
                return;
            }
            
            await canvasGroup.DOFade(targetAlpha, duration).WaitForCompletion(cancellation);
            
            canvasGroup.alpha = targetAlpha;
        }
    }
}
