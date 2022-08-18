using UnityEngine;

namespace MiniGames.Modules.Main.Menu.ChooseMenu
{
    public class CaseIsland : Island
    {
        [SerializeField] private GameObject particlesObject;

        private void Awake()
        {
            StopAnimation();
        }

        public override void StartAnimation() 
        {
            particlesObject.SetActive(true);
        }

        public override void StopAnimation() 
        {
            particlesObject.SetActive(false);
        }
    }
}

