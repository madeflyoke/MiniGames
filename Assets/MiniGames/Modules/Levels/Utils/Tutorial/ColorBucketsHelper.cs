using System;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace MiniGames.Modules.Level.Utils
{
    public class ColorBucketsHelper : TutorialHelper
    {
        private Image middleBucket;

        public void Initialize(Image middleBucket, Func<bool> stopTrigger)
        {
            base.Initialize(stopTrigger);
            this.middleBucket = middleBucket;     
        }

        public override async void ShowHelper()
        {
            helperPointers[0].position = middleBucket.GetComponent<DropZone>().CorrectObject.transform.position;
            helperPointers[0].gameObject.SetActive(true);
            Sequence seq = DOTween.Sequence(helperPointers[0]);
            seq.Append(helperPointers[0].DOMove(middleBucket.transform.position, 1.5f)
                .SetEase(Ease.Linear))
                .SetDelay(1f)
                .AppendInterval(1f)                  
                .SetLoops(-1, LoopType.Restart);
            await UniTask.WaitUntil(stopTrigger, cancellationToken: cts.Token);
            HideHelper();
        }
    }
}

