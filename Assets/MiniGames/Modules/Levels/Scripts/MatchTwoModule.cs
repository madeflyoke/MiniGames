using Cysharp.Threading.Tasks;
using MiniGames.Modules.Level.MatchTwo;
using MiniGames.Modules.Level.Utils;
using System.Threading;
using UnityEngine;

namespace MiniGames.Modules.Level
{
    public class MatchTwoModule : Module
    {
        [SerializeField] private MatchTwoController matchTwoController;
        [SerializeField] private Scratcher scratcher;
        private CancellationTokenSource cancellationToken;

        public override async void OnLoaded()
        {
            base.OnLoaded();
            cancellationToken = new CancellationTokenSource();
            await UniTask.Delay((int)(startGameDelay * 1000), cancellationToken: cancellationToken.Token);
            matchTwoController.StartGame();
        }

        private void OnEnable()
        {
            scratcher.exitButtonPressedEvent += UnloadPreparation;
            matchTwoController.BackToMenuSlider.exitSliderCompleteEvent += UnloadPreparation;
        }
        private void OnDisable()
        {
            scratcher.exitButtonPressedEvent -= UnloadPreparation;
            matchTwoController.BackToMenuSlider.exitSliderCompleteEvent -= UnloadPreparation;
        }

        protected override void UnloadPreparation()
        {
            base.UnloadPreparation();
        }
    }
}

