using MiniGames.Modules;
using MiniGames.Modules.Level;
using MiniGames.Modules.LoadingScreen;
using MiniGames.Modules.Main.Menu;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace MiniGames.Managers
{
    public class ModulesController : MonoBehaviour
    {
        [SerializeField] private MenuModule menuModule;
        [SerializeField] private MathModule mathModule;
        [SerializeField] private LoadingScreen loadingScreen; 
        private Module currentModule;
        private DiContainer container;
        private CancellationTokenSource cancellationToken;

        private void Awake()
        {
            cancellationToken = new CancellationTokenSource();
            menuModule.ChooseMenu.levelChosenEvent += LoadLevelModule;
            mathModule.backToMenuEvent += LoadMenuModule;
        }

        private void OnDisable() //never disabling?? i.e. full game lifetime and zenject access
        {
            menuModule.ChooseMenu.levelChosenEvent -= LoadLevelModule;
            mathModule.backToMenuEvent -= LoadMenuModule;
        }

        private async void LoadLevelModule(LevelModule levelType)
        {
            loadingScreen.StartAnimation();
            UnloadCurrentModule();
            switch (levelType)
            {
                case LevelModule.Math:
                    currentModule = container.InstantiatePrefab(mathModule.gameObject).GetComponent<Module>();
                    break;
                case LevelModule.MatchTwo:
                    break;
                case LevelModule.ColorBuckets:
                    break;
                case LevelModule.ChristmasTree:
                    break;
                case LevelModule.TOTEM_UNDEFINED:
                    break;
                default:
                    break;
            }
            await UniTask.Delay(5000, cancellationToken: cancellationToken.Token);
            loadingScreen.StopAnimation();
            currentModule.OnLoaded();
        }     

        private void LoadMenuModule()
        {

        }

        private async void UnloadCurrentModule()
        {
            currentModule.Unload();
        }
    }
}

