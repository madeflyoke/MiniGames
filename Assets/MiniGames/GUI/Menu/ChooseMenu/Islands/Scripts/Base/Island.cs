using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGames.GUI.Menu.ChooseMenu.Islands
{
    public abstract class Island : MonoBehaviour
    {
        [SerializeField] protected Button button;

        public Button Button => button;

        public virtual void StartAnimation() { }

        public virtual void StopAnimation() { }



    }

}
