using UnityEngine;

namespace MiniGames.Modules.Main.Menu.ChooseMenu
{
    public class TotemsIsland: Island
    {
        [SerializeField] private GameObject particlesObject;
        [SerializeField] private GameObject particlesObject_1;

        private void Awake()
        {
            StopAnimation();
        }

        public override void StartAnimation()
        {
            particlesObject.SetActive(true);
            particlesObject_1.SetActive(true);
        }

        public override void StopAnimation()
        {
            particlesObject.SetActive(false);
            particlesObject_1.SetActive(false);
        }
    }
}
