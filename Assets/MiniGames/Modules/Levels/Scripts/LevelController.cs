using MiniGames.Managers;
using MiniGames.Modules.Level.Utils;
using System;
using UnityEngine;
using Zenject;

namespace MiniGames.Modules.Level
{
    public abstract class LevelController : MonoBehaviour
    {
        [Inject] private PlayerPrefsData playerPrefsData;

        public event Action onExitEvent;

        [SerializeField] protected Scratcher scratcher;
        [SerializeField] protected BackToMenuSlider backToMenuSlider;
        protected LevelType levelType;
        protected bool isNeedReward;

        public virtual void Initialize(LevelType type)
        {
            levelType = type;
            isNeedReward = !playerPrefsData.IsLevelCompletedOnce[type];
        }

        protected virtual void OnEnable()
        {
            scratcher.exitButtonPressedEvent += OnExitScratching;
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

        private void OnExitScratching()
        {
            if (isNeedReward)
            {
                playerPrefsData.SaveFirstComplition(levelType);
            }
            OnExit();
        }

    }
}

