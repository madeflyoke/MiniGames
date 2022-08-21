using DG.Tweening;
using MiniGames.Modules.Level.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx.Triggers;
using UniRx;

namespace MiniGames.Modules.Level.XmasTree
{
    public class ToysBagController : MonoBehaviour
    {
        [SerializeField] private XmasTreeController xmasTreeController;
        [SerializeField] private Button button;
        [SerializeField] private Transform bagPointerHelper;
        [SerializeField] private RectTransform bagAnimationPivot;
        [SerializeField] private RectTransform startRevealPivot;
        [SerializeField] private RectTransform endRevealPivot;
        private Vector3 bagDefaultScale;
        private List<Draggable> toys;
        private XmasTreeController.StarData star;
        private CancellationTokenSource cancellationToken;
        private Draggable currentToy;

        private void Awake()
        {
            cancellationToken = new CancellationTokenSource();
            bagDefaultScale = bagAnimationPivot.localScale;
            toys = new();
        }

        public void Initialize()
        {
            foreach (var item in xmasTreeController.ToyCellPairs)
            {
                item.toy.gameObject.SetActive(false);
                item.toy.transform.position = startRevealPivot.transform.position;
                item.toy.DefaultPos = endRevealPivot.position;
                item.toyCell.correctAnswerEvent += NewToyPreparation;
                toys.Add(item.toy);
            }
            star = xmasTreeController.Star;
            star.starToy.gameObject.SetActive(false);
            star.starToy.transform.position = startRevealPivot.transform.position;
            star.starToy.DefaultPos = endRevealPivot.anchoredPosition;
            button.onClick.AddListener(ButtonListener);
            bagPointerHelper.gameObject.SetActive(false);
            Shuffle(ref toys);
        }

        public void ShowHelper()
        {
            bagPointerHelper.gameObject.SetActive(true);
            bagPointerHelper.DOMove(bagPointerHelper.position + (bagPointerHelper.up * 0.2f), 0.7f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
        public void HideHelper()
        {
            bagPointerHelper.DOKill();
            bagPointerHelper.gameObject.SetActive(false);
        }

        private void Shuffle(ref List<Draggable> list)
        {
            System.Random rnd = new();
            list = list.OrderBy(x => rnd.Next()).ToList();
        }

        private void ButtonListener()
        {
            button.interactable = false;
            xmasTreeController.Raycaster.enabled = false;
            if (bagPointerHelper.gameObject.activeInHierarchy == true)
            {
                HideHelper();
            }
            Animation();
        }

        private void Animation()
        {
            Sequence seq = DOTween.Sequence(); //bag animation
            seq.Append(bagAnimationPivot.DOScaleX(bagDefaultScale.x * 1.1f, 0.3f))
                .Join(bagAnimationPivot.DOScaleY(bagDefaultScale.y * 0.9f, 0.3f))
                .Append(bagAnimationPivot.DOScaleX(bagDefaultScale.x * 0.88f, 0.2f))
                .Join(bagAnimationPivot.DOScaleY(bagDefaultScale.y * 1.13f, 0.2f).OnComplete(() => ShowToy()))
                .Append(bagAnimationPivot.DOScaleX(bagDefaultScale.x, 0.8f))
                .Join(bagAnimationPivot.DOScaleY(bagDefaultScale.y, 0.8f)).SetEase(Ease.InCubic);
        }

        private void ShowToy()
        {
            if (toys[0] == null)
            {
                //end;
            }
            currentToy = toys[0];
            currentToy.gameObject.SetActive(true);
            currentToy.transform.DOMove(endRevealPivot.position, 0.8f).SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    xmasTreeController.Raycaster.enabled = true;
                    SupportToyAnimation();
                });
            toys.Remove(currentToy);
        }

        private void Update()
        {
            Debug.Log(Draggable.s_currentDraggable);
        }
        private void NewToyPreparation()
        {
            cancellationToken.Cancel();
            cancellationToken = new CancellationTokenSource();
            Draggable.s_currentDraggable = null;
            button.interactable = true; //end
        }

        private void SupportToyAnimation()
        {
            currentToy.transform.DOMove(currentToy.transform.position + (currentToy.transform.up * 0.2f), 0.7f)
                    .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

            currentToy.Image.OnEndDragAsObservable().RepeatUntilDisable(this).Subscribe(async x =>
            {
                while (Vector2.Distance(currentToy.transform.position, currentToy.DefaultPos) > 0.05f)
                {
                    await UniTask.Yield(cancellationToken.Token);
                }
                currentToy.transform.DOMove(currentToy.transform.position + (currentToy.transform.up * 0.2f), 0.7f)
               .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            }).AddTo(cancellationToken.Token);

        }
    }

}
