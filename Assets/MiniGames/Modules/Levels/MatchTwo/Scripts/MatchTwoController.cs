using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using System;

namespace MiniGames.Modules.Level.MatchTwo
{
    public class MatchTwoController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem choiceEffectPrefab;
        [SerializeField] private MatchTwoAnimator animator;
        [SerializeField] private int levelsCount;
        [SerializeField] private Transform pairArrivePivot;
        [SerializeField] private Transform suitcase;
        [SerializeField] private List<Sprite> items;
        [SerializeField] private List<Image> itemsPivots;
        private int currentLevelIndex;
        private GraphicRaycaster raycaster;
        private Image currentSelectable;
        private CancellationTokenSource cancellationToken;
        private Dictionary<Button, Image> itemsButtons;
        private int answersPerLevel;
        private ParticleSystem[] choiceParticles;
        private Dictionary<Transform, Vector3> defaultParticlesScales;

        private void Awake()
        {
            choiceParticles = new ParticleSystem[2];
            defaultParticlesScales = new();
            for (int i = 0; i < choiceParticles.Length; i++)
            {
                choiceParticles[i] = Instantiate(choiceEffectPrefab);
                defaultParticlesScales[choiceParticles[i].transform] = choiceParticles[i].transform.localScale;
            }
            answersPerLevel = itemsPivots.Count / 2;
            raycaster = GetComponent<GraphicRaycaster>();
            raycaster.enabled = false;
            animator.HideInstant();
            cancellationToken = new CancellationTokenSource();
            itemsButtons = new();
            foreach (var item in itemsPivots) //cache button components
            {
                itemsButtons[item.GetComponent<Button>()] = item;
            }
            SetupButtons();
            currentLevelIndex = 0;
            Shuffle(ref items);
            SetupItems();
        }

        private void Start()
        {
            animator.ShowStartAnimation(() =>
            {
                raycaster.enabled = true;
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
            Shuffle(ref selectedItems);

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
                item.Key.onClick.AddListener(() => ButtonListener(item.Value));
            }
        }

        private async void NextLevelPreparations()
        {
            itemsPivots[0].transform.parent.gameObject.SetActive(false);
            animator.SetCheckMark();
            currentLevelIndex++;
            if (currentLevelIndex >= levelsCount)
            {
                Debug.Log("endgame");
                return;
            }
            answersPerLevel = itemsPivots.Count / 2;
            SetupItems();
            foreach (var itemPivot in itemsPivots)
            {
                itemPivot.gameObject.SetActive(true);
            }
            await UniTask.Delay(1500, cancellationToken: cancellationToken.Token);
            animator.ShowNextAnimation(() =>
            {
                raycaster.enabled = true;
            });
        }

        private void SetChoiceParticles(int index, Transform tr)
        {
            choiceParticles[index].transform.position = new Vector3(tr.position.x, tr.position.y,
                40);
            choiceParticles[index].transform.parent = tr;
            choiceParticles[index].gameObject.SetActive(true);
            choiceParticles[index].Play();
        }

        private void ResetParticles()
        {
            foreach (var item in defaultParticlesScales)
            {
                item.Key.gameObject.SetActive(false);
                item.Key.parent = null;
                item.Key.localScale = item.Value; 
            }
        }

        private void ButtonListener(Image selectableImage) //first item selected-->cached in currentSelectable--->wait for second item to compare
        {
            if (currentSelectable == null) //if the item is first-selected
            {
                currentSelectable = selectableImage;
                SetChoiceParticles(0, currentSelectable.transform);
                currentSelectable.raycastTarget = false;
                currentSelectable.transform.DOScale(currentSelectable.transform.localScale * 1.2f, 0.3f);
            }
            else if (currentSelectable.sprite == selectableImage.sprite) //if second selected same as first
            {
                SetChoiceParticles(1, selectableImage.transform);
                raycaster.enabled = false;
                selectableImage.transform.DOScale(selectableImage.transform.localScale * 1.2f, 0.3f).OnComplete(() =>
                {                 
                    DOTween.Sequence()
                       .Append(currentSelectable.transform.DOMove(pairArrivePivot.position + Vector3.left / 2, 0.6f).SetEase(Ease.OutCubic)) //moving to arrive point 
                       .Join(selectableImage.transform.DOMove(pairArrivePivot.position + Vector3.right / 2, 0.6f).SetEase(Ease.OutCubic))
                       .Append(currentSelectable.transform.DOMove(suitcase.position, 0.3f)) //move down in suitcase
                       .Join(currentSelectable.transform.DOScale(0f, 0.3f))
                       .Join(selectableImage.transform.DOMove(suitcase.position, 0.3f))
                       .Join(selectableImage.transform.DOScale(0f, 0.3f))
                       .Insert(0.8f, suitcase.DOPunchScale(Vector3.one * 0.2f, 0.3f, 7).SetEase(Ease.InOutSine))
                       .OnComplete(() =>
                       {
                           choiceParticles[0].Stop();
                           choiceParticles[1].Stop();
                           ResetParticles();
                           currentSelectable.gameObject.SetActive(false);
                           selectableImage.gameObject.SetActive(false);
                           currentSelectable.raycastTarget = true;
                           currentSelectable = null;
                           answersPerLevel--;
                           if (answersPerLevel <= 0)
                           {
                               NextLevelPreparations();
                               return;
                           }
                           raycaster.enabled = true;
                       }).SetDelay(0.5f);
                });
                Debug.Log("yes");
            }
            else //if second selected different from first
            {
                choiceParticles[0].Stop();
                raycaster.enabled = false;
                currentSelectable.transform.DOScale(currentSelectable.transform.localScale / 1.2f, 0.3f)
                    .OnComplete(() =>
                    {
                        currentSelectable.raycastTarget = true;
                        selectableImage.raycastTarget = true;
                        currentSelectable = null;
                        raycaster.enabled = true;
                    });
                Debug.Log("no");
            }
        }

        private void Shuffle(ref List<Sprite> list)
        {
            System.Random rnd = new();
            list = list.OrderBy(x => rnd.Next()).ToList();
        }
    }
}

