using MiniGames.Modules.Level;
using MiniGames.Modules.Main;
using UnityEngine;

namespace MiniGames.Managers
{
    public class ModulesController : MonoBehaviour
    {
        [SerializeField] private MenuModule menuModule;
        [SerializeField] private MathModule mathModule;

        private void Awake()
        {
            
        }


        private void OnEnable()
        {
            menuModule.ChooseMenu.levelChosenEvent += LoadLevelModule;
        }
        private void OnDisable()
        {
            menuModule.ChooseMenu.levelChosenEvent -= LoadLevelModule;
        }

        private void LoadLevelModule(LevelModule levelType)
        {

        }     

        private void LoadMainModule(MainModule mainModule)
        {

        }

    }
}

