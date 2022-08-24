using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;

namespace MiniGames.Modules.Level.MatchTwo
{
    public class MatchTwoController : MonoBehaviour
    {
        [SerializeField] private MatchTwoAnimator animator;
        [SerializeField] private int levelsCount;
        [SerializeField] private List<Sprite> items;
        [SerializeField] private List<Image> itemsPivots;
        private int currentLevelIndex;
        private GraphicRaycaster raycaster;
        private Image currentSelectable;
        private CancellationTokenSource cancellationToken;
        private Dictionary<Button, Image> itemsButtons;

        private void Awake()
        {
            raycaster = GetComponent<GraphicRaycaster>();
            animator.HideInstant();
            cancellationToken = new CancellationTokenSource();
            itemsButtons = new();
            foreach (var item in itemsPivots) //cache button components
            {
                itemsButtons[item.GetComponent<Button>()] = item;
            }
            SetupButtons();
            currentLevelIndex = 1;
            Shuffle(ref items);
            SetupItems();
        }

        private void Start()
        {
            animator.ShowAnimation(null);
        }

        private void SetupItems() //each wave
        {
            List<Sprite> selectedItems = new();
            int max = items.Count / levelsCount;
            for (int i = 0; i < max; i++) //list of doubled items
            {
                selectedItems.Add(items[i]);
                selectedItems.Add(items[i]);
                items.Remove(items[i]);
            }
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

        private void ButtonListener(Image image)
        {
            if (currentSelectable == null) //if first item selected
            {
                currentSelectable = image;
                image.raycastTarget = false;
                image.transform.DOScale(image.transform.localScale * 1.2f, 0.2f).OnComplete(() =>
                {
                    Debug.Log("null assignation");
                });
            }
            else if (currentSelectable.sprite == image.sprite) //if second selected same as first
            {
                Debug.Log("yes");
            }
            else //if second selected different from first
            {
                raycaster.enabled = false;
                currentSelectable.transform.DOScale(currentSelectable.transform.localScale / 1.2f, 0.2f)
                    .OnComplete(() =>
                    {
                        currentSelectable.raycastTarget = true;
                        image.raycastTarget = true;
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

