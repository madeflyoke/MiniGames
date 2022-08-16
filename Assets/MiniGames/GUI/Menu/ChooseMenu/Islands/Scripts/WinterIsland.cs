using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGames.GUI.Menu.ChooseMenu.Islands
{
    public class WinterIsland : Island
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
