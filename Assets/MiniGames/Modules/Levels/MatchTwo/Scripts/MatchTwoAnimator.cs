using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace MiniGames.Modules.Level.MatchTwo
{
    public class MatchTwoAnimator : MonoBehaviour
    {
        public struct ScaleAndPosition
        {
            public Vector3 scale;
            public Vector3 position;
        }

        [SerializeField] private float showingDuration;
        [SerializeField] private float spaceDuration;
        [SerializeField] private float lineSpaceDuration;
        [SerializeField] private float suitcaseDelay;
        [Space]
        [SerializeField] private List<GameObject> checkMarks;
        [SerializeField] private List<Transform> orderedPivots;
        [SerializeField] private Transform suitcase;
        private Dictionary<Transform, ScaleAndPosition> defaultPivotsValues;


        public void Awake()
        {
            defaultPivotsValues = new();
            foreach (var item in orderedPivots)
            {
                defaultPivotsValues[item] = new ScaleAndPosition { scale = item.localScale, position = item.position };
            }
        }

        public void ShowStartAnimation(Action onComplete)
        {
            foreach (var item in checkMarks)
            {
                item.SetActive(false);
            }
            Sequence seq = DOTween.Sequence();
            for (int i = 0; i < orderedPivots.Count; i++)
            {
                orderedPivots[i].localScale = Vector3.zero;
                seq.Append(orderedPivots[i].DOScale(defaultPivotsValues[orderedPivots[i]].scale, showingDuration).SetEase(Ease.InOutSine)
                    .SetDelay(i==0?0:(i%(orderedPivots.Count/3)==0?lineSpaceDuration:spaceDuration))); //first without delay, each line - some more delay
            }
            Vector3 caseDefaultScale = suitcase.localScale;
            Vector3 caseDefaultPos = suitcase.position;
            suitcase.localScale = Vector3.zero;
            suitcase.position = caseDefaultPos + Vector3.up * 7f;
            seq.Append(suitcase.DOScale(caseDefaultScale, 0.2f).SetEase(Ease.InOutElastic, -1).SetDelay(suitcaseDelay))
                .Append(suitcase.DOMove(caseDefaultPos, 1.2f).SetEase(Ease.OutBounce)).SetDelay(0.3f)
                .OnComplete(() => onComplete?.Invoke());

            orderedPivots[0].parent.gameObject.SetActive(true);
            suitcase.gameObject.SetActive(true);
        }

        public void SetCheckMark()
        {
            for (int i = 0; i < checkMarks.Count; i++)
            {
                if (checkMarks[i].activeInHierarchy == false)
                {
                    checkMarks[i].SetActive(true);
                    checkMarks[i].transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, vibrato: 1);
                    return;
                }
            }
        }

        private void SetDefaultPivotsPositions()
        {
            foreach (var item in orderedPivots)
            {
                item.transform.position = defaultPivotsValues[item].position;
            }
        }

        public void ShowNextAnimation(Action onComplete)
        {
            SetDefaultPivotsPositions();
            Sequence seq = DOTween.Sequence();
            for (int i = 0; i < orderedPivots.Count; i++)
            {
                orderedPivots[i].localScale = Vector3.zero;
                seq.Append(orderedPivots[i].DOScale(defaultPivotsValues[orderedPivots[i]].scale, showingDuration).SetEase(Ease.InOutSine)
                    .SetDelay(i == 0 ? 0 : (i % (orderedPivots.Count / 3) == 0 ? lineSpaceDuration : spaceDuration)))
                    .OnComplete(() => onComplete?.Invoke());
            }
            orderedPivots[0].parent.gameObject.SetActive(true);
        }

        public void HideInstant()
        {
            orderedPivots[0].parent.gameObject.SetActive(false);
            suitcase.gameObject.SetActive(false);
        }

    }
}

