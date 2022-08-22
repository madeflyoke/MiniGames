using MiniGames.Modules.Level.Utils;
using MiniGames.Modules.Level.XmasTree;
using UnityEngine;

namespace MiniGames.Modules.Level
{
    public class XmasTreeModule : Module
    {
        [SerializeField] private Scratcher scratcher;
        [SerializeField] private XmasTreeController xmasTreeController;

        public override void OnLoaded()
        {
            base.OnLoaded();
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
