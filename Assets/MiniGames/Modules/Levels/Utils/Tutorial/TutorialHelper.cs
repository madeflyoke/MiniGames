using UnityEngine;
using System.Threading;
using System;
using System.Collections.Generic;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace MiniGames.Modules.Level.Utils
{
    public class TutorialHelper : MonoBehaviour
    {
        [SerializeField] protected List<Transform> helperPointers;

        public Func<bool> DefaultStopTrigger { get; private set; } = () => Input.GetKeyDown(KeyCode.Mouse0);
        protected CancellationTokenSource cts;
        protected Func<bool> stopTrigger;

        public virtual void Initialize(Func<bool> stopTrigger)
        {
            cts = new();
            foreach (var item in helperPointers)
            {
                item.gameObject.SetActive(false);
            }
            this.stopTrigger = stopTrigger;
        }

        public virtual async void ShowHelper()
        {
            foreach (var item in helperPointers)
            {
                item.gameObject.SetActive(true);
                item.DOMove(item.position + (item.up * 0.2f), 0.7f)
                .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            }
            await UniTask.WaitUntil(stopTrigger, cancellationToken: cts.Token);
            HideHelper();
        }

        protected virtual void HideHelper()
        {
            foreach (var item in helperPointers)
            {
                item.DOKill();
                item.gameObject.SetActive(false);
            }
        }

        protected virtual void OnDestroy()
        {
            cts.Cancel();
            HideHelper();
        }
    }
}

