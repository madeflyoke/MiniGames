using MiniGames.Modules;
using MiniGames.Modules.LoadingScreen;
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
        [SerializeField] private GameObject matchTwoModulePrefab;
        [SerializeField] private GameObject colorBucketsModulePrefab;
        [SerializeField] private GameObject totemsModulePrefab;
        [SerializeField] private LoadingScreen loadingScreen;
        private Module currentModule;
        private CancellationTokenSource cts;

        private void Awake()
        {
            cts = new CancellationTokenSource();
        }

        private void Start()
        {
            LoadMenuModule(MenuModule.Mode.MainMenu);
        }
        
        private async void LoadLevelModule(LevelType levelType)
        {
            loadingScreen.StartAnimation();
            UnloadCurrentModule();
            switch (levelType)
            {
                case LevelType.Math:
                    currentModule = container.InstantiatePrefab(mathModulePrefab.gameObject).GetComponent<Module>();
                    break;
                case LevelType.MatchTwo:
                    currentModule = container.InstantiatePrefab(matchTwoModulePrefab.gameObject).GetComponent<Module>();
                    break;
                case LevelType.ColorBuckets:
                    currentModule = container.InstantiatePrefab(colorBucketsModulePrefab.gameObject).GetComponent<Module>();
                    break;
                case LevelType.XmasTree:
                    currentModule = container.InstantiatePrefab(xmasTreeModulePrefab.gameObject).GetComponent<Module>();
                    break;
                case LevelType.Totems:
                    currentModule = container.InstantiatePrefab(totemsModulePrefab.gameObject).GetComponent<Module>();
                    break;
                default:
                    break;
            }
            LevelModule component = (LevelModule)currentModule;
            component.LevelController.Initialize(levelType);
            component.backToMenuEvent += () => LoadMenuModule(MenuModule.Mode.ChooseMenu);            
            await UniTask.Delay(3000, cancellationToken: cts.Token);
            loadingScreen.StopAnimation();
            component.OnLoaded();
        }

        private async void LoadMenuModule(MenuModule.Mode mode)
        {
            loadingScreen.StartAnimation();
            UnloadCurrentModule();
            currentModule = container.InstantiatePrefab(menuModulePrefab).GetComponent<Module>();
            MenuModule menuComponent = currentModule.GetComponent<MenuModule>();
            menuComponent.Initialize(mode);
            menuComponent.ChooseMenuController.levelChosenEvent += LoadLevelModule;
            await UniTask.Delay(3000, cancellationToken: cts.Token);
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

