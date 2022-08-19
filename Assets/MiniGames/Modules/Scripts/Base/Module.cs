using MiniGames.Managers;
using UnityEngine;

namespace MiniGames.Modules
{
    public abstract class Module : MonoBehaviour
    {
        public bool onLoaded { get; protected set; }

        public virtual void OnLoaded()
        {

        }

        public virtual void Load()
        {

        }      

        public virtual void Unload()
        {
            Destroy(gameObject);
        }
    }

}
