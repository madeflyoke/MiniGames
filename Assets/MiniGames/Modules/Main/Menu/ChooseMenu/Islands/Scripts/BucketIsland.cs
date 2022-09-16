using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading;

namespace MiniGames.Modules.Main.Menu.ChooseMenu
{
    public class BucketIsland : Island
    {
        [Header("Ordered list")]
        [Space]
        [SerializeField] private List<Transform> buckets;
        [SerializeField] private float showDuration;
        private Dictionary<Transform, Vector3> defaultsPos;

        private void Awake()
        {
            defaultsPos = new();
            foreach (var item in buckets)
            {
                defaultsPos[item] = item.position;
            }
        }

        public override void StartAnimation()
        {
            foreach (var item in buckets)
            {
                item.position -= (item.up * 7f);
            }
            Sequence seq = DOTween.Sequence(transform);
            seq.SetDelay(1.5f);
            for (int i = 0; i < buckets.Count; i++)
            {
                seq.Append(buckets[i].DOMove(defaultsPos[buckets[i]], showDuration)
                    .SetEase(Ease.OutCirc));
            }
            for (int i = 0; i < buckets.Count; i++)
            {
                seq.Append(buckets[i].DOMove(defaultsPos[buckets[i]] - (buckets[i].up * 7f), showDuration * 0.6f)
                    .SetEase(Ease.InCirc).SetDelay(i == 0 ? 1.5f : 0f));
            }
            seq.SetLoops(-1, LoopType.Restart);
        }

        public override void StopAnimation()
        {
            transform.DOKill();
            foreach (var item in buckets)
            {
                item.DOKill();
            }
        }

        private void OnDestroy()
        {
            StopAnimation();
        }
    }

}
