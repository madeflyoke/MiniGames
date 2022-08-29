using MiniGames.Modules.Level;
using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace MiniGames.Modules
{
    public class LevelModule : Module
    {
        public event Action backToMenuEvent;

        [SerializeField] private float startGameDelay;
        [SerializeField] private LevelController levelController;
        private CancellationTokenSource cancellationToken;
        public LevelController LevelController => levelController;

        public async void OnLoaded()
        {
            cancellationToken = new CancellationTokenSource();
            await UniTask.Delay((int)(startGameDelay * 1000), cancellationToken: cancellationToken.Token);
            levelController.StartGame();
        }

        private void OnEnable()
        {
            levelController.onExitEvent += OnExit;
        }
        private void OnDisable()
        {
            levelController.onExitEvent -= OnExit;
        }

        private void OnExit()
        {
            backToMenuEvent?.Invoke();
        }
    }
}

