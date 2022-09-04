using System;
using UnityEngine;

namespace MiniGames.Modules
{
    public abstract class Module : MonoBehaviour
    {        
        public virtual void Unload()
        {
            Destroy(gameObject);
        }
    }

}
