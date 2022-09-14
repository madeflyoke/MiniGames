using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace MiniGames.Modules.Level.ColorBuckets
{
    public class ColorBucketsAnimator : MonoBehaviour
    {
        [Header("Ordered lists")]
        [Space]
        [SerializeField] private List<Transform> bucketsPivots;
        [SerializeField] private List<Transform> toysPivots;
        [SerializeField] private List<GameObject> checkMarks;
        private Dictionary<Transform, Vector3> bucketsDefaultScales;
        private CancellationTokenSource cts;

        private void Awake()
        {
            cts = new();
            bucketsDefaultScales = new();
        }

        private void Start()
        {        
            foreach (var item in bucketsPivots)
            {
                bucketsDefaultScales[item] = item.localScale;
                item.localScale = Vector3.zero;
            }
            foreach (var item in toysPivots)
            {
                item.gameObject.SetActive(false);
            }
            foreach (var item in checkMarks)
            {
                item.SetActive(false);
            }
        }

        public async void ShowAnimation()
        {
            await UniTask.Delay(1000, cancellationToken: cts.Token);
            foreach (var item in bucketsPivots)
            {
                await UniTask.Delay(450, cancellationToken: cts.Token);
                item.DOScale(bucketsDefaultScales[item], 0.5f).SetEase(Ease.InOutElastic, -1);
            }
            await UniTask.Delay(700, cancellationToken: cts.Token);
            foreach (var item in toysPivots)
            {
                await UniTask.Delay(300, cancellationToken: cts.Token);
                Vector3 defScale = item.localScale;
                item.localScale = Vector3.zero;
                item.gameObject.SetActive(true);
                item.DOScale(defScale, 0.5f).SetEase(Ease.InOutElastic, -1);
            }
            await UniTask.Delay(500, cancellationToken: cts.Token);
        }

        public async void HideAnimation(Action onComplete)
        {
            await UniTask.Delay(500, cancellationToken: cts.Token);
            foreach (var item in bucketsPivots)
            {
                item.DOScale(item.localScale * 1.1f, 0.35f).OnComplete(() =>
                {
                    item.DOScale(Vector3.zero, 0.08f).SetEase(Ease.Flash);
                });
                
            }
            await UniTask.Delay(500, cancellationToken: cts.Token);
            onComplete?.Invoke();
        }

        public void SetCheckMark()
        {
            for (int i = 0; i < checkMarks.Count; i++)
            {
                if (checkMarks[i].activeInHierarchy == false)
                {
                    checkMarks[i].SetActive(true);
                    checkMarks[i].transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, vibrato: 1);
                    return;
                }
            }
        }
    }

}
