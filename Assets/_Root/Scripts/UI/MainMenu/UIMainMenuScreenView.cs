using System;
using Pet.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Pet.MainMenu
{
    public class UIMainMenuScreenView : UIScreenViewBase
    {
        [SerializeField] private Button playButton;

        private Action onPlayRequested;

        private void Awake()
        {
            playButton.onClick.AddListener(NotifyPlayRequested);
        }

        public void SetCallbacks(Action playRequested)
        {
            onPlayRequested = playRequested;
        }

        private void NotifyPlayRequested()
        {
            onPlayRequested?.Invoke();
        }
    }
}
