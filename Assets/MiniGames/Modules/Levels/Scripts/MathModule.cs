using MiniGames.Modules.Level.Math;
using MiniGames.Modules.Level.Utils;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace MiniGames.Modules.Level
{
    public class MathModule : Module
    {
        [SerializeField] private MathController mathController;
        [SerializeField] private Scratcher scratcher;
        private CancellationTokenSource cancellationToken;

        public override async void OnLoaded()
        {
            base.OnLoaded();
            cancellationToken = new CancellationTokenSource();
            await UniTask.Delay((int)(startGameDelay * 1000), cancellationToken: cancellationToken.Token);
            mathController.StartGame();
        }

        private void OnEnable()
        {
            scratcher.exitButtonPressedEvent += UnloadPreparation;
            mathController.BackToMenuSlider.exitSliderCompleteEvent += UnloadPreparation;
        }
        private void OnDisable()
        {
            scratcher.exitButtonPressedEvent -= UnloadPreparation;
            mathController.BackToMenuSlider.exitSliderCompleteEvent -= UnloadPreparation;
        }

        protected override void UnloadPreparation()
        {
            base.UnloadPreparation();
        }
    }
}

