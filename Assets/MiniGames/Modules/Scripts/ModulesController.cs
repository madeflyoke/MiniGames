using MiniGames.Modules;
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
        [Inject] private DiContainer container;

        [SerializeField] private GameObject menuModulePrefab;
        [SerializeField] private GameObject mathModulePrefab;
        [SerializeField] private GameObject xmasTreeModulePrefab;
        [SerializeField] private LoadingScreen loadingScreen;
        private Module currentModule;
        private CancellationTokenSource cancellationToken;

        private void Awake()
        {
            cancellationToken = new CancellationTokenSource();
        }

        private void Start()
        {
            LoadMenuModule(MenuModule.Mode.MainMenu);
        }

        private async void LoadLevelModule(LevelModule levelType)
        {
            loadingScreen.StartAnimation();
            UnloadCurrentModule();
            switch (levelType)
            {
                case LevelModule.Math:
                    currentModule = container.InstantiatePrefab(mathModulePrefab.gameObject).GetComponent<Module>();
                    break;
                case LevelModule.MatchTwo:
                    break;
                case LevelModule.ColorBuckets:
                    break;
                case LevelModule.XmasTree:
                    currentModule = container.InstantiatePrefab(xmasTreeModulePrefab.gameObject).GetComponent<Module>();
                    break;
                case LevelModule.TOTEM_UNDEFINED:
                    break;
                default:
                    break;
            }
            var component = currentModule.GetComponent<Module>();
            component.backToMenuEvent += () => LoadMenuModule(MenuModule.Mode.ChooseMenu);
            await UniTask.Delay(3000, cancellationToken: cancellationToken.Token);
            loadingScreen.StopAnimation();
            currentModule.OnLoaded();
        }

        private async void LoadMenuModule(MenuModule.Mode mode)
        {
            loadingScreen.StartAnimation();
            UnloadCurrentModule();
            currentModule = container.InstantiatePrefab(menuModulePrefab).GetComponent<Module>();
            MenuModule menuComponent = currentModule.GetComponent<MenuModule>();
            menuComponent.Initialize(mode);
            menuComponent.ChooseMenuController.levelChosenEvent += LoadLevelModule;
            await UniTask.Delay(3000, cancellationToken: cancellationToken.Token);
            loadingScreen.StopAnimation();
        }

        private void UnloadCurrentModule()
        {
            if (currentModule != null)
            {
                currentModule.Unload();
            }
        }
    }
}

