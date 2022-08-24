using MiniGames.Modules.Level.Utils;
using MiniGames.Modules.Level.XmasTree;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace MiniGames.Modules.Level
{
    public class XmasTreeModule : Module
    {
        [SerializeField] private Scratcher scratcher;
        [SerializeField] private XmasTreeController xmasTreeController;
        private CancellationTokenSource cancellationToken;

        public override async void OnLoaded()
        {
            base.OnLoaded();
            cancellationToken = new CancellationTokenSource();
            await UniTask.Delay((int)(startGameDelay * 1000), cancellationToken: cancellationToken.Token);
            xmasTreeController.StartGame();
        }

        private void OnEnable()
        {
            xmasTreeController.BackToMenuSlider.exitSliderCompleteEvent += UnloadPreparation;
            scratcher.exitButtonPressedEvent += UnloadPreparation;
        }
        private void OnDisable()
        {
            scratcher.exitButtonPressedEvent -= UnloadPreparation;
            xmasTreeController.BackToMenuSlider.exitSliderCompleteEvent -= UnloadPreparation;
        }

        protected override void UnloadPreparation()
        {
            base.UnloadPreparation();
        }
    }

}
