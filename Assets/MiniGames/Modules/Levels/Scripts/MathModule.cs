using MiniGames.Modules.Level.Math;
using MiniGames.Modules.Level.Utils;
using UnityEngine;

namespace MiniGames.Modules.Level
{
    public class MathModule : Module
    {
        [SerializeField] private MathController mathController;
        [SerializeField] private Scratcher scratcher;

        public override void OnLoaded()
        {
            base.OnLoaded();
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

