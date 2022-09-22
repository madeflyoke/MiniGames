using MiniGames.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;

namespace MiniGames.Modules.Main.Menu
{
    public class PrizesController : MonoBehaviour
    {
        [Inject] private PlayerPrefsData playerPrefsData;

        [Serializable]
        public struct PrizeByLevel
        {
            public LevelType levelType;
            public Image prizeImage;
        }

        [SerializeField] private Button prizesButton;
        [SerializeField] private GameObject prizesScrollView;
        [SerializeField] private Color lockedColor;
        [SerializeField] private Color unlockedColor;
        [SerializeField] private List<PrizeByLevel> prizesByLevels;
        [SerializeField] private Transform fromPos;
        [SerializeField] private Transform toPos;
        private Vector3 defaultScrollViewScale;

        private void Awake()
        {
            defaultScrollViewScale = prizesScrollView.transform.localScale;
            prizesScrollView.SetActive(false);
            prizesScrollView.transform.position = fromPos.position;
            prizesScrollView.transform.localScale = Vector3.zero;
            foreach (var item in prizesByLevels) //set unlock if level was completed at least once
            {
                if (playerPrefsData.IsLevelCompletedOnce[item.levelType]==true)
                {
                    item.prizeImage.color = unlockedColor;
                }
                else
                {
                    item.prizeImage.color = lockedColor;
                }
            }
        }

        private void OnEnable()
        {
            prizesButton.onClick.AddListener(ButtonListener);

        }
        private void OnDisable()
        {
            prizesButton.onClick.RemoveListener(ButtonListener);
        }

        private void ButtonListener()
        {
            prizesButton.interactable = false;
            if (prizesScrollView.activeInHierarchy == false)
            {
                DOTween.Sequence()
               .Append(prizesScrollView.transform.DOMove(toPos.position, 0.35f))
               .Join(prizesScrollView.transform.DOScale(defaultScrollViewScale, 0.35f)).SetEase(Ease.InQuad)
               .OnComplete(()=> prizesButton.interactable = true);
                prizesScrollView.SetActive(true);
            }
            else
            {
                DOTween.Sequence()
               .Append(prizesScrollView.transform.DOMove(fromPos.position, 0.35f))
               .Join(prizesScrollView.transform.DOScale(0f, 0.35f)).SetEase(Ease.InQuad)
               .OnComplete(() =>
               {
                   prizesScrollView.SetActive(false);
                   prizesButton.interactable = true;
               });             
            }       
        }

    }
}

