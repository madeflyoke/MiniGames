using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace MiniGames.Modules.Level.MatchTwo
{
    public class MatchTwoAnimator : MonoBehaviour
    {
        [SerializeField] private float showingDuration;
        [SerializeField] private float spaceDuration;
        [SerializeField] private float lineSpaceDuration;
        [SerializeField] private float suitcaseDelay;

        [SerializeField] private Transform itemsParent;
        [SerializeField] private List<Transform> orderedPivots;
        [SerializeField] private Transform suitcase;

        public void ShowAnimation(Action onComplete)
        {
            Sequence seq = DOTween.Sequence();

            for (int i = 0; i < orderedPivots.Count; i++)
            {
                Vector3 pivotDefaultScale = orderedPivots[i].localScale;
                orderedPivots[i].localScale = Vector3.zero;
                seq.Append(orderedPivots[i].DOScale(pivotDefaultScale, showingDuration).SetEase(Ease.InOutSine)
                    .SetDelay(i==0?0:(i%(orderedPivots.Count/3)==0?lineSpaceDuration:spaceDuration))); //first without delay, each line - some more delay
            }
            Vector3 caseDefaultScale = suitcase.localScale;
            Vector3 caseDefaultPos = suitcase.position;
            suitcase.localScale = Vector3.zero;
            suitcase.position = caseDefaultPos + Vector3.up * 7f;
            seq.Append(suitcase.DOScale(caseDefaultScale, 0.2f).SetEase(Ease.InOutElastic, - 1).SetDelay(suitcaseDelay))
                .Append(suitcase.DOMove(caseDefaultPos, 1.2f).SetEase(Ease.OutBounce)).SetDelay(0.3f);
                
            itemsParent.gameObject.SetActive(true);
            suitcase.gameObject.SetActive(true);
        }

        public void HideInstant()
        {
            itemsParent.gameObject.SetActive(false);
            suitcase.gameObject.SetActive(false);
        }

    }
}

