using System;
using UnityEngine;
using DG.Tweening;

namespace MiniGames.Modules.Main.Menu.ChooseMenu
{
    public class ChooseMenuController : MonoBehaviour
    {
        public event Action<LevelModule> levelChosenEvent;

        //[SerializeField] private Button mainMenuButton; 
        [SerializeField] private GameObject wavesParticles;
        [SerializeField] private Transform islandsPivot;
        [SerializeField] private Island housesIsland;
        [SerializeField] private Island caseIsland;
        [SerializeField] private Island winterIsland;


        private void Start()
        {
            islandsPivot.gameObject.SetActive(false);
        }

        public void Initialize()
        {
            islandsPivot.gameObject.SetActive(true);
            wavesParticles.SetActive(true);
            housesIsland.StartAnimation();
            caseIsland.StartAnimation();
            winterIsland.StartAnimation();
            islandsPivot.DOPunchScale(Vector3.one*0.05f, 0.3f,5).OnComplete(() =>
            {               
                SetupButton(housesIsland, LevelModule.Math);
            });              
        }

        private void SetupButton(Island island, LevelModule levelType)
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
