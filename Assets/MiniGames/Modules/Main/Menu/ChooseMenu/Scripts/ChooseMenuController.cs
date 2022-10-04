using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MiniGames.Modules.Main.Menu.ChooseMenu
{
    public class ChooseMenuController : MonoBehaviour
    {
        public event Action backToMenuEvent;
        public event Action<LevelType> levelChosenEvent;

        [SerializeField] private Button mainMenuButton;

        [SerializeField] private GameObject background;
        [SerializeField] private GameObject wavesParticles;
        [SerializeField] private Transform islandsPivot;
        [SerializeField] private Island housesIsland;
        [SerializeField] private Island caseIsland;
        [SerializeField] private Island winterIsland;
        [SerializeField] private Island bucketsIsland;
        [SerializeField] private Island totemsIsland;

        public void Initialize(MenuModule.Mode mode)
        {
            switch (mode)
            {
                case MenuModule.Mode.MainMenu:
                    Deactivate();
                    break;
                case MenuModule.Mode.ChooseMenu:
                    Activate();
                    break;
                default:
                    break;
            }
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            background.gameObject.SetActive(true);
            mainMenuButton.gameObject.SetActive(false);
            islandsPivot.gameObject.SetActive(false);
            wavesParticles.SetActive(false);
            StopAnimationIslands();
        }

        public void Activate()
        {
            background.gameObject.SetActive(true);
            islandsPivot.gameObject.SetActive(true);
            mainMenuButton.gameObject.SetActive(true);
            wavesParticles.SetActive(true);
            mainMenuButton.transform.DOPunchScale(Vector3.one * 0.05f, 0.3f, 5).OnComplete(() =>
            {
                mainMenuButton.interactable = true;
                mainMenuButton.onClick.AddListener(MainMenuButtonListener);
            });
            StartAnimationIslands();
            islandsPivot.DOPunchScale(Vector3.one * 0.05f, 0.3f, 5).OnComplete(() => //set level types to islands here
            {
                SetupIslandButton(housesIsland, LevelType.Math);
                SetupIslandButton(winterIsland, LevelType.XmasTree);
                SetupIslandButton(caseIsland, LevelType.MatchTwo);
                SetupIslandButton(bucketsIsland, LevelType.ColorBuckets);
                SetupIslandButton(totemsIsland, LevelType.Totems);
            });
        }

        private void MainMenuButtonListener()
        {
            mainMenuButton.interactable = false;
            RemoveButtonsListeners();
            mainMenuButton.gameObject.SetActive(false);
            backToMenuEvent?.Invoke();
            TurnOffIslands();
        }

        private void StartAnimationIslands()
        {
            housesIsland.StartAnimation();
            caseIsland.StartAnimation();
            winterIsland.StartAnimation();
            bucketsIsland.StartAnimation();
            totemsIsland.StartAnimation();
        }
        private void StopAnimationIslands()
        {
            housesIsland.StopAnimation();
            caseIsland.StopAnimation();
            winterIsland.StopAnimation();
            bucketsIsland.StopAnimation();
            totemsIsland.StopAnimation();
        }

        private void TurnOffIslands()
        {
            StopAnimationIslands();
            islandsPivot.DOPunchScale(Vector3.one * 0.1f, 0.4f, 5).OnComplete(() =>
            {
                wavesParticles.SetActive(false);
                islandsPivot.gameObject.SetActive(false);
            });
        }

        private void RemoveButtonsListeners()
        {
            housesIsland.Button.onClick.RemoveAllListeners();
            mainMenuButton.onClick.RemoveAllListeners();
        }

        private void SetupIslandButton(Island island, LevelType levelType)
        {
            island.Button.onClick.AddListener(() =>
            {
                island.Button.interactable = false;
                island.transform.DOPunchScale(Vector2.one * 0.1f, 0.3f, 5).OnComplete(() =>
                {
                    levelChosenEvent?.Invoke(levelType);
                });
            });
        }
    }
}
