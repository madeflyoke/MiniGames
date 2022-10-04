using Cysharp.Threading.Tasks;
using DG.Tweening;
using MiniGames.Modules.Level.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGames.Modules.Level.Utils
{
    public class TotemsHelper : TutorialHelper
    {
        private Transform pointFrom;
        private Transform pointTo;
        private Func<bool> firstPointerStopTrigger;
        private Func<bool> secondPointerStopTrigger;

        private void Awake()
        {
            cts = new();
            foreach (var item in helperPointers)
            {
                item.gameObject.SetActive(false);
            }
        }

        public void InitializeFirstPointer(Func<bool> stopTrigger)
        {
            firstPointerStopTrigger = stopTrigger;
        }

        public void InitializeSecondPointer(Transform pointFrom, Transform pointTo, Func<bool> stopTrigger)
        {
            secondPointerStopTrigger = stopTrigger;
            this.pointFrom = pointFrom;
            this.pointTo = pointTo;
        }

        public override async void ShowHelper()
        {
            helperPointers[0].gameObject.SetActive(true);
            helperPointers[0].DOMove(helperPointers[0].position + (helperPointers[0].up * 0.2f), 0.7f)
                .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

            await UniTask.WaitUntil(firstPointerStopTrigger, cancellationToken: cts.Token);
            HideHelper();
            ShowSecondHelper();
        }

        private async void ShowSecondHelper()
        {
            await UniTask.Delay(2000, cancellationToken: cts.Token);
            helperPointers[1].position = pointFrom.position;
            helperPointers[1].gameObject.SetActive(true);
            Sequence seq = DOTween.Sequence(helperPointers[1]);
            seq.Append(helperPointers[1].DOMove(pointTo.position, 1.5f)
                .SetEase(Ease.Linear))
                .SetDelay(1f)
                .AppendInterval(1f)
                .SetLoops(-1, LoopType.Restart);
            await UniTask.WaitUntil(secondPointerStopTrigger, cancellationToken: cts.Token);
            HideHelper();
        }
    }
}
