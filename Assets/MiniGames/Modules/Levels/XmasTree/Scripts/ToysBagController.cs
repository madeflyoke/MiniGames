using DG.Tweening;
using MiniGames.Modules.Level.Utils;
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
        [Header("Particles")]
        [SerializeField] private ParticleSystem pushEffect;
        [SerializeField] private ParticleSystem correctAnswerEffectPrefab;
        [Space]
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
        private Dictionary<Draggable, ParticleSystem> answersEffects; 

        private void Awake()
        {
            cancellationToken = new CancellationTokenSource();
            bagDefaultScale = bagAnimationPivot.localScale;
            toys = new();
            answersEffects = new();
            pushEffect.gameObject.SetActive(false);
        }

        public void Initialize()
        {       
            foreach (var item in xmasTreeController.ToyCellPairs)
            {
                item.toy.gameObject.SetActive(false);
                item.toy.transform.position = startRevealPivot.transform.position;
                item.toy.DefaultPos = endRevealPivot.position;
                item.toyCell.correctAnswerEvent += NewToyPreparation;
                var particle = Instantiate(correctAnswerEffectPrefab,
                    item.toyCell.transform.position + Vector3.back * 10f, correctAnswerEffectPrefab.transform.rotation);
                particle.gameObject.SetActive(false);
                answersEffects[item.toy] = particle;
                toys.Add(item.toy);
            }
            star = xmasTreeController.Star;
            star.starToy.gameObject.SetActive(false);
            star.starToy.transform.position = startRevealPivot.transform.position;
            star.starToy.DefaultPos = endRevealPivot.position;
            star.starCell.correctAnswerEvent += () =>
            {
                cancellationToken.Cancel();
                Draggable.s_currentDraggable = null;
            };
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
            pushEffect.gameObject.SetActive(true);
            pushEffect.Play();
            if (toys.Count == 0)
            {
                currentToy = star.starToy;
            }
            else
            {
                currentToy = toys[0];
                toys.Remove(currentToy);
            }           
            currentToy.gameObject.SetActive(true);
            currentToy.transform.DOMove(endRevealPivot.position, 0.8f).SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    xmasTreeController.Raycaster.enabled = true;
                    SupportToyAnimation();
                });
        }

        private void NewToyPreparation()
        {         
            cancellationToken.Cancel();
            cancellationToken = new CancellationTokenSource();
            answersEffects[currentToy].gameObject.SetActive(true);
            answersEffects[currentToy].Play();
            Draggable.s_currentDraggable = null;
            button.interactable = true; //end
        }

        private void SupportToyAnimation()
        {
            currentToy.transform.DOMove(currentToy.transform.position + (currentToy.transform.up * 0.3f), 0.6f)
                    .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

            currentToy.Image.OnEndDragAsObservable().RepeatUntilDisable(this).Subscribe(async x =>
            {
                await UniTask.WaitUntil(() => currentToy.Image.raycastTarget == true, 
                    cancellationToken: cancellationToken.Token);
                currentToy.transform.DOMove(currentToy.transform.position + (currentToy.transform.up * 0.3f), 0.6f)
               .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            }).AddTo(cancellationToken.Token);

        }

    }
}
