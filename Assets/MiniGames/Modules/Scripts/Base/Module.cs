using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace MiniGames.Modules
{
    public abstract class Module : MonoBehaviour
    {
        public event Action backToMenuEvent;

        [Header("Module")]
        [SerializeField] private float startGameDelay;
        protected CancellationTokenSource cancellationToken;

        public virtual async void OnLoaded()
        {
            await UniTask.Delay((int)(startGameDelay * 1000), cancellationToken: cancellationToken.Token);
        }

        public virtual void Load()
        {

        }      

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
