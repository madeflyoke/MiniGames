using MiniGames.Modules.Level.Utils;
using System;
using UnityEngine;

namespace MiniGames.Modules.Level
{
    public abstract class LevelController : MonoBehaviour
    {
        public event Action onExitEvent;

        [SerializeField] protected Scratcher scratcher;
        [SerializeField] protected BackToMenuSlider backToMenuSlider;

        protected virtual void OnEnable()
        {
            scratcher.exitButtonPressedEvent += OnExit;
            backToMenuSlider.exitSliderCompleteEvent += OnExit;
        }
        protected virtual void OnDisable() //if problems --> check for unsubscribing
        {
           
        }

        public abstract void StartGame();

        private void OnExit() //can be protected and virtual if needed
        {
            onExitEvent?.Invoke();
        }

    }
}

