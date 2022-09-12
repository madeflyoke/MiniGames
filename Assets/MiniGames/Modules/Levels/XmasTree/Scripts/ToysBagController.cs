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
using MiniGames.Extensions;

namespace MiniGames.Modules.Level.XmasTree
{
    public class ToysBagController : MonoBehaviour
    {
        [Header("Particles")]
        [SerializeField] private ParticleSystem pushEffect;
        [SerializeField] private ParticleSystem correctAnswerEffectPrefab;
        [Space]
        [SerializeField] private XmasTreeController xmasTreeController;
        [SerializeField] private TutorialHelper tutorialHelper;
        [SerializeField] private Button button;
        [SerializeField] private RectTransform bagAnimationPivot;
        [SerializeField] private RectTransform startRevealPivot;
        [SerializeField] private RectTransform endRevealPivot;
        private Vector3 bagDefaultScale;
        private List<Draggable> toys;
        private XmasTreeController.StarData star;
        private CancellationTokenSource cts;
        private Draggable currentToy;
        private Dictionary<Draggable, ParticleSystem> answersEffects; 

        private void Awake()
        {
            cts = new CancellationTokenSource();
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
                cts.Cancel();
            };
            button.onClick.AddListener(ButtonListener);
            toys = toys.Shuffle();
            tutorialHelper.Initialize(() => button.interactable == false);
        }

        public void ShowHelper()
        {
            tutorialHelper.ShowHelper();
        }

        private void ButtonListener()
        {
            button.interactable = false;
            Animation();
        }

        private void Animation()
        {
            Sequence seq = DOTween.Sequence(); //bag animation
            seq.Append(bagAnimationPivot.DOScaleX(bagDefaultScale.x * 1.1f, 0.27f))
                .Join(bagAnimationPivot.DOScaleY(bagDefaultScale.y * 0.9f, 0.27f))
                .Append(bagAnimationPivot.DOScaleX(bagDefaultScale.x * 0.88f, 0.17f))
                .Join(bagAnimationPivot.DOScaleY(bagDefaultScale.y * 1.13f, 0.17f).OnComplete(() => ShowToy()))
                .Append(bagAnimationPivot.DOScaleX(bagDefaultScale.x, 0.72f))
                .Join(bagAnimationPivot.DOScaleY(bagDefaultScale.y, 0.72f)).SetEase(Ease.InCubic);
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
            currentToy.transform.SetAsLastSibling();
            currentToy.gameObject.SetActive(true);
            currentToy.transform.DOMove(endRevealPivot.position, 0.72f).SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    SupportToyAnimation();
                });
        }

        private void NewToyPreparation()
        {         
            cts.Cancel();
            cts = new CancellationTokenSource();
            answersEffects[currentToy].gameObject.SetActive(true);
            answersEffects[currentToy].Play();
            button.interactable = true; //end
        }

        private void SupportToyAnimation()
        {
            currentToy.transform.DOMove(currentToy.transform.position + (currentToy.transform.up * 0.3f), 0.6f)
                    .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

            currentToy.Image.OnEndDragAsObservable().RepeatUntilDisable(this).Subscribe(async x =>
            {
                await UniTask.WaitUntil(() => currentToy.Image.raycastTarget == true, 
                    cancellationToken: cts.Token);
                currentToy.transform.DOMove(currentToy.transform.position + (currentToy.transform.up * 0.3f), 0.6f)
               .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            }).AddTo(cts.Token);        
        }

        private void OnDestroy()
        {
            foreach (var item in answersEffects)
            {
                Destroy(item.Value.gameObject);
            }
            if (currentToy!=null)
            {
                currentToy.transform.DOKill();
            }
            cts.Cancel();
        }

    }
}
