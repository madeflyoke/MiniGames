using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace MiniGames.Modules.Level.Math
{
    public class MathAnimator : MonoBehaviour
    {
        [Header("Parts")]
        [SerializeField] private MathQuestion mathQuestion;
        [SerializeField] private GameObject answerZone;
        [SerializeField] private List<MathAnswerVariant> orderedVariants;
        [SerializeField] private List<GameObject> checkMarks;
        [Space]
        [SerializeField] private float hidingSpeedTime;
        [SerializeField] private float showSpeedTime;
        [SerializeField] private float showSpaceTime;

        private struct MathQuestionScales
        {
            public KeyValuePair<Transform, Vector3> leftDefaultScale;
            public KeyValuePair<Transform, Vector3> mathOperatorDefaultScale;
            public KeyValuePair<Transform, Vector3> rightDefaultScale;
            public KeyValuePair<Transform, Vector3> equalMarkDefaultScale;
        }

        private MathQuestionScales questionScales; //1
        private Vector3 answerZoneDefaultScale; //2
        private Dictionary<MathAnswerVariant, Vector3> mathAnswersDefaultScales; //3

        private void Awake()
        {          
            questionScales = new MathQuestionScales
            {
                leftDefaultScale = new(mathQuestion.Left.transform, mathQuestion.Left.transform.localScale),
                mathOperatorDefaultScale = new(mathQuestion.MathOperator.transform, mathQuestion.MathOperator.transform.localScale),
                rightDefaultScale = new(mathQuestion.Right.transform, mathQuestion.Right.transform.localScale),
                equalMarkDefaultScale = new(mathQuestion.EqualMark.transform, mathQuestion.EqualMark.transform.localScale)
            };
            answerZoneDefaultScale = answerZone.transform.localScale;
            mathAnswersDefaultScales = new();
            foreach (var item in orderedVariants)
            {
                mathAnswersDefaultScales.Add(item, item.transform.localScale);
            }
            foreach (var item in checkMarks)
            {
                item.SetActive(false);
            }
        }

        private void Start()
        {
            HideInstant();
        }

        public void SetCheckMark()
        {
            for (int i = 0; i < checkMarks.Count; i++)
            {
                if (checkMarks[i].activeInHierarchy==false)
                {
                    checkMarks[i].SetActive(true);
                    checkMarks[i].transform.DOPunchScale(Vector3.one * 0.3f, 0.2f,vibrato:1);
                    return;
                }
            }
        }

        public void ShowingAnimation(Action onComplete)
        {
            Sequence mySequence = DOTween.Sequence();
            mySequence
                .Append(questionScales.leftDefaultScale.Key.DOScale(questionScales.leftDefaultScale.Value, showSpeedTime))
                .Append(questionScales.mathOperatorDefaultScale.Key.DOScale(questionScales.mathOperatorDefaultScale.Value, showSpeedTime).SetDelay(showSpaceTime))
                .Append(questionScales.rightDefaultScale.Key.DOScale(questionScales.rightDefaultScale.Value, showSpeedTime).SetDelay(showSpaceTime))
                .Append(questionScales.equalMarkDefaultScale.Key.DOScale(questionScales.equalMarkDefaultScale.Value, showSpeedTime).SetDelay(showSpaceTime))
                .Append(answerZone.transform.DOScale(answerZoneDefaultScale, showSpeedTime).SetDelay(showSpaceTime).OnComplete(()=>
                {
                    for (int i = 0; i < mathAnswersDefaultScales.Count; i++)
                    {
                        orderedVariants[i].transform.localScale = Vector3.zero;
                        orderedVariants[i].gameObject.SetActive(true);
                        if (i == (mathAnswersDefaultScales.Count - 1))//last animation to callback
                        {
                            orderedVariants[i].transform.DOScale(mathAnswersDefaultScales[orderedVariants[i]], showSpeedTime)
                        .SetDelay(i * showSpaceTime * 0.7f + 1f).OnComplete(() => onComplete?.Invoke());
                            continue;
                        }
                        orderedVariants[i].transform.DOScale(mathAnswersDefaultScales[orderedVariants[i]], showSpeedTime)
                        .SetDelay(i * showSpaceTime * 0.7f + 1f);
                    }
                }));                    
        }

        public void HidingAnimation(Action onComplete)
        {
            Sequence mySequence = DOTween.Sequence();
            mySequence
                .Join(questionScales.leftDefaultScale.Key.DOScale(0, hidingSpeedTime))
                .Join(questionScales.mathOperatorDefaultScale.Key.DOScale(0, hidingSpeedTime))
                .Join(questionScales.rightDefaultScale.Key.DOScale(0, hidingSpeedTime))
                .Join(questionScales.equalMarkDefaultScale.Key.DOScale(0, hidingSpeedTime))
                .Join(answerZone.transform.DOScale(0, hidingSpeedTime))
                .OnStart(() =>
                {
                    foreach (var item in orderedVariants)
                    {
                        item.transform.DOScale(0, hidingSpeedTime);
                        item.gameObject.SetActive(false);
                    }
                }).OnComplete(()=>onComplete?.Invoke());      
        }

        private void HideInstant()
        {
            questionScales.leftDefaultScale.Key.localScale = Vector3.zero;
            questionScales.mathOperatorDefaultScale.Key.localScale = Vector3.zero;
            questionScales.rightDefaultScale.Key.localScale = Vector3.zero;
            questionScales.equalMarkDefaultScale.Key.localScale = Vector3.zero;

            answerZone.transform.localScale = Vector3.zero;
            foreach (var item in orderedVariants)
            {
                item.transform.localScale = Vector3.zero;
            }
        }
    }
}

