using UnityEngine;
using UnityEngine.UI;

namespace MiniGames.Modules.Main.Menu.ChooseMenu
{
    public abstract class Island : MonoBehaviour
    {
        [SerializeField] protected Button button;

        public Button Button => button;

        public virtual void StartAnimation() { }

        public virtual void StopAnimation() { }



    }

}
