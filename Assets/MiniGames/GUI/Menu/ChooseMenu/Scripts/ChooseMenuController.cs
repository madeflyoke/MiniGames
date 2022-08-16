using MiniGames.GUI.Menu.ChooseMenu.Islands;
using System;
using UnityEngine;
using DG.Tweening;

namespace MiniGames.GUI.Menu.ChooseMenu
{
    public class ChooseMenuController : MonoBehaviour
    {
        public event Action<LevelModule> levelChosenEvent;

        //[SerializeField] private Button mainMenuButton; 
        [SerializeField] private GameObject wavesParticles;

        [SerializeField] private Island housesIsland;
        [SerializeField] private Island caseIsland;
        [SerializeField] private Island winterIsland;

        public void Awake()
        {
            wavesParticles.SetActive(true);
            SetupButtons(housesIsland,caseIsland,winterIsland);
        }

        private void OnEnable()
        {
            
        }

        private void SetupButtons(params Island[] islands)
        {
            foreach (var item in islands)
            {
                item.StartAnimation();
                item.Button.onClick.AddListener(() => {
                    item.transform.DOPunchScale(Vector2.one * 0.1f, 0.3f,10);
                    item.Button.onClick.RemoveAllListeners();
                });
            }         
        }
    }
}
