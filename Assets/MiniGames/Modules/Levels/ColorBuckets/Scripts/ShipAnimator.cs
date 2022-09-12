using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace MiniGames.Modules.Level.ColorBuckets
{
    public class ShipAnimator : MonoBehaviour
    {
        [Header("Smoke")]
        [SerializeField] private Image smoke;
        [SerializeField] private Transform smokePivot;
        [SerializeField] private float smokeDelay;
        [SerializeField] private float smokeSpeedMulitplier;
        [Header("Ship")]
        [SerializeField] private Transform ship;
        [SerializeField] private Transform endPoint;
        [SerializeField] private float shipTripDuration;
        [SerializeField] private float nextTripDelay;
        private CancellationTokenSource cts;

        private void Awake()
        {
            cts = new();
            smoke.transform.position = smokePivot.position;
        }

        private void Start()
        {
            StartAnimation();
        }

        public void StartAnimation()
        {
            ShipTrip();
            SmokeAnimation();
        }

        private async void ShipTrip()
        {
            Vector3 defPos = ship.position;
            while (true)
            {
                ship.position = defPos;
                await ship.DOMoveX(endPoint.transform.position.x, shipTripDuration).SetEase(Ease.Linear).AsyncWaitForCompletion().AsUniTask();
                await UniTask.Delay((int)(nextTripDelay * 1000), cancellationToken: cts.Token);
            }
        }

        private async void SmokeAnimation()
        {
            Color defColor = smoke.color;
            Vector3 defScale = smoke.transform.localScale;
            while (true)
            {
                smoke.transform.localScale = Vector3.zero;
                smoke.color = defColor;
                await DOTween.Sequence(transform)
                .Append(smoke.transform.DOScale(defScale, 2f * smokeSpeedMulitplier))
                .Join(smoke.DOFade(0, 1.5f * smokeSpeedMulitplier).SetDelay(0.8f))
                .AsyncWaitForCompletion().AsUniTask();
                await UniTask.Delay((int)(smokeDelay * 1000), cancellationToken: cts.Token);
                smoke.transform.position = smokePivot.position;
            }
        }

        private void OnDestroy()
        {
            cts.Cancel();
            transform.DOKill();
            ship.DOKill();
        }
    }

}
