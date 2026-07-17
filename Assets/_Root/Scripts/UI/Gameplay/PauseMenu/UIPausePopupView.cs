using System;
using Pet.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Pet.Gameplay
{
    public class UIPausePopupView : UIPopupViewBase
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button backToMenuButton;

        private Action onResumeRequested;
        private Action onReturnToMenuRequested;

        private void Awake()
        {
            resumeButton.onClick.AddListener(NotifyResumeRequested);
            backToMenuButton.onClick.AddListener(NotifyReturnToMenuRequested);
        }

        public void SetCallbacks(Action resumeRequested, Action returnToMenuRequested)
        {
            onResumeRequested = resumeRequested;
            onReturnToMenuRequested = returnToMenuRequested;
        }

        private void NotifyResumeRequested()
        {
            onResumeRequested?.Invoke();
        }

        private void NotifyReturnToMenuRequested()
        {
            onReturnToMenuRequested?.Invoke();
        }
    }
}
