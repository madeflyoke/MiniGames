using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using MiniGames.Modules.Level.Utils;
using MiniGames.Extensions;

namespace MiniGames.Modules.Level.MatchTwo
{
    public class MatchTwoController : LevelController
    {
        [SerializeField] private MatchTwoAnimator animator;
        [SerializeField] private ScratchAgainButton scratchAgainButton;
        [SerializeField] private MatchTwoHelper helper;
        [Space]
        [SerializeField] private ParticleSystem winEffect;
        [SerializeField] private ParticleSystem choiceEffectPrefab;
        [Tooltip("Set value manually depending on items formation i.e. rows, columns, overall items count etc.")]
        [SerializeField] private int levelsCount;
        [SerializeField] private Transform pairArrivePivot;
        [SerializeField] private Transform suitcase;
        [SerializeField] private List<Sprite> items;
        [SerializeField] private List<Image> itemsPivots;
        private int currentLevelIndex;
        private GraphicRaycaster raycaster;
        private Image currentSelectable;
        private CancellationTokenSource cts;
        private Dictionary<Button, Image> itemsButtons;
        private int answersPerLevel;
        private Queue<ParticleSystem> choiceParticles;
        private Dictionary<Transform, ParticleSystem> activeChoiceParticles;
        private Dictionary<Transform, Vector3> defaultParticlesScales;

        private void Awake()
        {
            winEffect.gameObject.SetActive(false);
            choiceParticles = new();
            activeChoiceParticles = new();
            defaultParticlesScales = new();
            for (int i = 0; i < 12; i++) //choice particles pool
            {
                var particle = Instantiate(choiceEffectPrefab);
                particle.gameObject.SetActive(false);
                choiceParticles.Enqueue(particle);
                defaultParticlesScales[particle.transform] = particle.transform.localScale;
            }
            answersPerLevel = itemsPivots.Count / 2;
            raycaster = GetComponent<GraphicRaycaster>();
            raycaster.enabled = false;
            animator.HideInstant();
            cts = new CancellationTokenSource();
            itemsButtons = new();
            foreach (var item in itemsPivots) //cache button components
            {
                itemsButtons[item.GetComponent<Button>()] = item;
            }
            SetupButtons();
            currentLevelIndex = 0;
            items = items.Shuffle();
        }

        public override void StartGame()
        {
            SetupItems();
            helper.Initialize(() => currentSelectable != null, itemsPivots);
            animator.ShowStartAnimation(() =>
            {
                raycaster.enabled = true;
                backToMenuSlider.gameObject.SetActive(true);
                helper.ShowHelper();
            });
        }

        private void SetupItems() //each wave(level)
        {
            List<Sprite> selectedItems = new();
            int max = items.Count / (levelsCount - currentLevelIndex);
            for (int i = 0; i < max; i++) //list of doubled items
            {
                selectedItems.Add(items[i]);
                selectedItems.Add(items[i]);
            }
            items.RemoveRange(0, max);
            selectedItems = selectedItems.Shuffle();
            int index = 0;
            foreach (var itemPivot in itemsPivots) //setup pivot sprites by items
            {
                if (selectedItems.Count <= index)
                {
                    break;
                }
                itemPivot.sprite = selectedItems[index];
                itemPivot.name = itemPivot.sprite.name;
                index++;
            }
        }

        private void SetupButtons()
        {
            foreach (var item in itemsButtons)
            {
                item.Key.onClick.AddListener(() => ItemButtonListener(item.Value));
            }
        }

        private async void NextLevelPreparations()
        {
            itemsPivots[0].transform.parent.gameObject.SetActive(false);
            animator.SetCheckMark();
            currentLevelIndex++;
            if (currentLevelIndex >= levelsCount) //end
            {
                EndLogic();
                return;
            }
            answersPerLevel = itemsPivots.Count / 2;
            SetupItems();
            foreach (var itemPivot in itemsPivots)
            {
                itemPivot.gameObject.SetActive(true);
            }
            await UniTask.Delay(1500, cancellationToken: cts.Token);
            animator.ShowNextAnimation(() =>
            {
                raycaster.enabled = true;
            });
        }

        private ParticleSystem SetChoiceParticles(Transform tr)
        {
            if (choiceParticles.Peek() == null)
            {
                var additionalParticle = Instantiate(choiceEffectPrefab);
                additionalParticle.gameObject.SetActive(false);
                choiceParticles.Enqueue(additionalParticle);
                defaultParticlesScales[additionalParticle.transform] = additionalParticle.transform.localScale;
            }
            var particle = choiceParticles.Dequeue();
            activeChoiceParticles[tr] = particle;
            particle.transform.localScale = defaultParticlesScales[particle.transform];
            particle.transform.position = new Vector3(tr.position.x, tr.position.y, 40);
            particle.transform.parent = tr;
            particle.gameObject.SetActive(true);
            particle.Play();
            return particle;
        }

        private void ResetChoiceParticles(params Transform[] tr)
        {
            for (int i = 0; i < tr.Length; i++)
            {
                var particle = activeChoiceParticles[tr[i]];
                particle.transform.parent = null;
                particle.Stop();
                choiceParticles.Enqueue(particle);
            }
        }

        private void ItemButtonListener(Image selectableImage) //first item selected-->cached in currentSelectable--->wait for second item to compare
        {
            if (currentSelectable == null) //if the item is first-selected
            {
                currentSelectable = selectableImage;
                SetChoiceParticles(currentSelectable.transform);
                currentSelectable.raycastTarget = false;
                currentSelectable.transform.DOScale(currentSelectable.transform.localScale * 1.2f, 0.3f);
            }
            else if (currentSelectable.sprite == selectableImage.sprite) //if second selected same as first
            {
                CorrectPairItems(currentSelectable, selectableImage);
            }
            else //if second selected different from first
            {
                ResetChoiceParticles(currentSelectable.transform);
                var item = currentSelectable;
                currentSelectable = null;
                item.transform.DOScale(item.transform.localScale / 1.2f, 0.2f)
                    .OnComplete(() =>
                    {
                        item.raycastTarget = true;
                    });
            }
        }

        private void CorrectPairItems(Image firstItem, Image secondItem)
        {
            currentSelectable = null;
            secondItem.raycastTarget = false;
            SetChoiceParticles(secondItem.transform);
            secondItem.transform.DOScale(secondItem.transform.localScale * 1.2f, 0.3f).OnComplete(() =>
            {
                suitcase.DOKill();
                DOTween.Sequence(transform)
                   .Append(firstItem.transform.DOMove(pairArrivePivot.position + Vector3.left / 2, 0.5f).SetEase(Ease.OutCubic)) //moving to arrive point 
                   .Join(secondItem.transform.DOMove(pairArrivePivot.position + Vector3.right / 2, 0.5f).SetEase(Ease.OutCubic))
                   .Append(firstItem.transform.DOMove(suitcase.position, 0.3f)) //move down in suitcase
                   .Join(firstItem.transform.DOScale(0f, 0.3f))
                   .Join(secondItem.transform.DOMove(suitcase.position, 0.3f))
                   .Join(secondItem.transform.DOScale(0f, 0.3f))
                   .Insert(0.8f, suitcase.DOPunchScale(Vector3.one * 0.2f, 0.3f, 7).SetEase(Ease.InOutSine))
                   .OnComplete(() =>
                   {
                       ResetChoiceParticles(firstItem.transform, secondItem.transform);
                       firstItem.gameObject.SetActive(false);
                       secondItem.gameObject.SetActive(false);
                       firstItem.raycastTarget = true;
                       secondItem.raycastTarget = true;
                       answersPerLevel--;
                       if (answersPerLevel <= 0)
                       {
                           NextLevelPreparations();
                           return;
                       }
                   }).SetDelay(0.3f);
            });
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
                raycaster.enabled = true;
                scratchAgainButton.Activate();
                await UniTask.WaitUntil(() => scratchAgainButton.Button.interactable == false, cancellationToken: cts.Token);
            }
            await UniTask.Delay(3000, cancellationToken: cts.Token);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            transform.DOKill();
            foreach (var itemPivot in itemsPivots)
            {
                itemPivot.transform.DOKill();
            }
            if (cts != null)
            {
                cts.Cancel();
            }
            foreach (var item in choiceParticles)
            {
                if (item.gameObject != null)
                {
                    Destroy(item.gameObject);
                }
            }
            suitcase.DOKill();
        }
    }
}

