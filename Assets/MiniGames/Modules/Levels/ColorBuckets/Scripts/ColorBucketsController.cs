using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using MiniGames.Extensions;
using MiniGames.Modules.Level.Utils;
using DG.Tweening;
using UniRx.Triggers;
using UniRx;
using System.Threading;
using System;

namespace MiniGames.Modules.Level.ColorBuckets
{
    public class ColorBucketsController : LevelController
    {
        private const int _levelsCount = 5;

        [SerializeField] private ColorBucketsAnimator animator;
        [SerializeField] private ParticleSystem winEffect;
        [SerializeField] private ParticleSystem correctAnswerEffect;
        [SerializeField] private ScratchAgainButton scratchAgainButton;
        [SerializeField] private List<Image> buckets;
        [SerializeField] private List<Image> toysPivots;
        [SerializeField] private List<Color> mainColors;
        [SerializeField] private List<Sprite> toys;
        private List<List<Color>> previousColorSets;
        private Dictionary<Image, DropZone> bucketsDropZones;
        private Dictionary<DropZone, ParticleSystem> bucketsParticles;
        private int maxAnswers;
        private CancellationTokenSource cts;

        private void Awake()
        {
            winEffect.gameObject.SetActive(false);
            cts = new();
            maxAnswers = _levelsCount * buckets.Count;
            previousColorSets = new();
            bucketsDropZones = new();
            bucketsParticles = new();
            foreach (var item in buckets)
            {
                DropZone dropZone = item.GetComponent<DropZone>();
                var particle = Instantiate(
                    correctAnswerEffect, item.transform.position+(Vector3.back*20f), correctAnswerEffect.transform.rotation);
                particle.gameObject.SetActive(false);
                bucketsParticles[dropZone] = particle;
                bucketsDropZones[item] = dropZone;
                dropZone.correctAnswerEvent += CheckAnswersRow;
                dropZone.correctAnswerEvent += () =>
                {
                    particle.gameObject.SetActive(true);
                    particle.Play();
                };
            }
            toys = toys.Shuffle();
            SetAnswers();
        }

        public override void StartGame()
        {
            animator.ShowAnimation();
            SupportToysAnimation();
        }

        private void CheckAnswersRow()
        {         
            maxAnswers--;
            if (maxAnswers==0)
            {
                animator.SetCheckMark();
                EndLogic();
                return;
            }
            else if (maxAnswers%buckets.Count==0)
            {
                cts.Cancel();
                cts = new();
                animator.HideAnimation(() =>
                {
                    animator.SetCheckMark();
                    SetAnswers();
                    animator.ShowAnimation();
                    SupportToysAnimation();
                });
            }
        }

        private async void SetAnswers()
        {
            SetToys();
            List<Color> currentColors=new(mainColors);
            List<Color> finalColors;
            do
            {
                currentColors = currentColors.Shuffle();
                finalColors = CorrectColorSet(currentColors);
                await UniTask.Yield();
            }            
            while (finalColors == null);
            previousColorSets.Add(finalColors);
            buckets = buckets.Shuffle();
            toysPivots = toysPivots.Shuffle();
            for (int i = 0; i < buckets.Count; i++)
            {
                buckets[i].color = finalColors[i];
                toysPivots[i].color = finalColors[i];
                bucketsDropZones[buckets[i]].Initialize(toysPivots[i].GetComponent<Draggable>());
            }
        }

        private List<Color> CorrectColorSet(List<Color> colors)
        {
            var firstThree = colors.GetRange(0, 3);
            if (previousColorSets.Count == 0)
            {
                return firstThree;
            }
            foreach (var item in previousColorSets)
            {
                if (firstThree.SequenceEqual(item)==true)
                {
                    return null;
                }
            }
            return firstThree;
        }

        private void SetToys()
        {
            for (int i = 0; i < toysPivots.Count; i++)
            {
                toysPivots[i].sprite = toys[^1];
                toys.Remove(toys[^1]);
            }
        }

        private void SupportToysAnimation()
        {
            foreach (var item in toysPivots)
            {
                item.transform.DOMove(item.transform.position + (item.transform.up * 0.3f), 0.6f)
                   .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
                item.OnEndDragAsObservable().RepeatUntilDisable(this).Subscribe(async x =>
                {
                    await UniTask.WaitUntil(() => item.raycastTarget == true,
                        cancellationToken: cts.Token);
                    item.transform.DOMove(item.transform.position + (item.transform.up * 0.3f), 0.6f)
                   .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
                }).AddTo(cts.Token);
            }
        }

        private async void EndLogic()
        {
            if (isNeedReward)
                backToMenuSlider.gameObject.SetActive(false);
            winEffect.gameObject.SetActive(true);
            winEffect.Play();
            await UniTask.WaitUntil(() => winEffect.gameObject.activeInHierarchy == false, cancellationToken: cts.Token);
            if (isNeedReward)
                scratcher.StartScratching();
            else
            {
                scratchAgainButton.Activate();
                await UniTask.WaitUntil(() => scratchAgainButton.Button.interactable == false, cancellationToken: cts.Token);
            }
            await UniTask.Delay(3000, cancellationToken: cts.Token);
            gameObject.SetActive(false);
        }
    }
}
