using MiniGames.Modules.Level.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGames.Modules.Level
{
    public class XmasTreeModule : Module
    {
        public event Action backToMenuEvent;

        [SerializeField] private Scratcher scratcher;

        public override void OnLoaded()
        {
            base.OnLoaded();
            //mathController.StartGame();
        }

        private void OnEnable()
        {
            scratcher.exitButtonPressedEvent += UnloadPreparation;
        }
        private void OnDisable()
        {
            scratcher.exitButtonPressedEvent -= UnloadPreparation;
        }

        private void UnloadPreparation()
        {
            backToMenuEvent?.Invoke();
        }
    }

}
