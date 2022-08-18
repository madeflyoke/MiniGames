using MiniGames.Modules.Main.Menu.ChooseMenu;
using UnityEngine;

namespace MiniGames.Modules.Main.Menu
{
    public class MenuModule : Module
    {
        [SerializeField] private MainMenuController mainMenuController;
        [SerializeField] private ChooseMenuController chooseMenu;
        public ChooseMenuController ChooseMenu => chooseMenu;

        public override void Load()
        {
            
        }

        private void OnEnable()
        {
            mainMenuController.transitionOutCompleteEvent += ShowChooseMenu;
        }
        private void OnDisable()
        {
            mainMenuController.transitionOutCompleteEvent -= ShowChooseMenu;
        }

        private void ShowChooseMenu()
        {
            chooseMenu.Initialize();
        }

        private void ShowMainMenu()
        {

        }
    }

}

