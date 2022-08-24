using System;
using UnityEngine;

namespace MiniGames.Modules
{
    public abstract class Module : MonoBehaviour
    {
        public event Action backToMenuEvent;

        [Header("Module")]
        [SerializeField] protected float startGameDelay;

        public virtual void OnLoaded() { }

        public virtual void Load() { }      

        protected virtual void UnloadPreparation()
        {
            backToMenuEvent?.Invoke();
        }

        public virtual void Unload()
        {
            Destroy(gameObject);
        }
    }

}
