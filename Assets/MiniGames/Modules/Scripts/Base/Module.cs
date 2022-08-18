using MiniGames.Managers;
using UnityEngine;

namespace MiniGames.Modules
{
    public abstract class Module : MonoBehaviour
    {

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
