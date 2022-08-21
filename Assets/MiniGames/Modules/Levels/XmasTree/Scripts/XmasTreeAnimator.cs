using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace MiniGames.Modules.Level.XmasTree
{
    public class XmasTreeAnimator : MonoBehaviour
    {
        [Header("Tree Animation")]
        [SerializeField] private RectTransform tree;
        [SerializeField] private float jumpHeight;
        [SerializeField] private float jumpDuration;
        [Header("Toys Cells Animation")]
        [SerializeField] private XmasTreeController xmasTreeController;
        [SerializeField] private float showSpeedTime;
        [SerializeField] private float showSpaceTime;
        private Dictionary<Transform, Vector3> defaultToysPivotsScales;
        private KeyValuePair<Transform, Vector3> defaultStarPivotScale;

        public void Initialize()
        {
            SetupDefaultScales();
            HideInstant();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                
            }
        }

        private void SetupDefaultScales()
        {
            defaultToysPivotsScales = new();
            foreach (var item in xmasTreeController.ToysPivots)
            {
                defaultToysPivotsScales[item] = item.transform.localScale;
            }
            defaultStarPivotScale = new KeyValuePair<Transform, Vector3>(xmasTreeController.Star.starPivot,
                xmasTreeController.Star.starPivot.localScale);
        }


        public void ShowingAnimation(Action onComplete)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(defaultStarPivotScale.Key.DOScale(defaultStarPivotScale.Value, showSpeedTime));
            foreach (var item in defaultToysPivotsScales)
            {
                seq.Append(item.Key.DOScale(item.Value.x, showSpeedTime).SetDelay(showSpaceTime));
            }
            seq.Append(tree.DOJumpAnchorPos(tree.anchoredPosition, jumpHeight, 1, jumpDuration)
                .SetEase(Ease.InOutSine).OnComplete(() => onComplete?.Invoke()));
         
        }

        private void HideInstant()
        {
            foreach (var item in defaultToysPivotsScales)
            {
                item.Key.localScale = Vector3.zero;
            }
            defaultStarPivotScale.Key.localScale = Vector3.zero;
        }
    }

}
