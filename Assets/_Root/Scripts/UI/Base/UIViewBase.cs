using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pet.UI
{
    public abstract class UIViewBase : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        public bool IsVisible { get; private set; }

        public virtual UniTask ShowAsync(CancellationToken cancellation = default)
        {
            cancellation.ThrowIfCancellationRequested();
            SetVisibleState(true);
            return UniTask.CompletedTask;
        }

        public virtual UniTask HideAsync(CancellationToken cancellation = default)
        {
            cancellation.ThrowIfCancellationRequested();
            SetVisibleState(false);
            return UniTask.CompletedTask;
        }

        public void ShowInstant()
        {
            SetVisibleState(true);
        }

        public void HideInstant()
        {
            SetVisibleState(false);
        }

        protected void SetVisibleState(bool visible)
        {
            IsVisible = visible;
            gameObject.SetActive(true);
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.blocksRaycasts = visible;
            canvasGroup.interactable = visible;

            if (!visible)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
