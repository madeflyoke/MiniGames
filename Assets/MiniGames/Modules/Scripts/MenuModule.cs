using MiniGames.Modules.Main.Menu;
using MiniGames.Modules.Main.Menu.ChooseMenu;
using UnityEngine;

namespace MiniGames.Modules
{
    public class MenuModule : Module
    {
        public enum Mode
        {
            MainMenu,
            ChooseMenu
        }

        [SerializeField] private MainMenuController mainMenuController;
        [SerializeField] private ChooseMenuController chooseMenuController;
        public ChooseMenuController ChooseMenuController => chooseMenuController;

        public void Initialize(Mode mode)
        {
            chooseMenuController.Initialize(mode);
            mainMenuController.Initialize(mode);
        }

        private void OnEnable()
        {
            mainMenuController.transitionOutCompleteEvent += ChooseMenuLogic;
            mainMenuController.transitionInCompleteEvent += MainMenuLogic;
            chooseMenuController.backToMenuEvent += mainMenuController.StartZoomOut;
        }

        private void OnDisable()
        {
            mainMenuController.transitionOutCompleteEvent -= ChooseMenuLogic;
            mainMenuController.transitionInCompleteEvent -= MainMenuLogic;
            chooseMenuController.backToMenuEvent -= mainMenuController.StartZoomOut;
        }

        private void ChooseMenuLogic()
        {
            chooseMenuController.Activate();
        }

        private void MainMenuLogic()
        {
            chooseMenuController.Deactivate();
        }
    }

}

