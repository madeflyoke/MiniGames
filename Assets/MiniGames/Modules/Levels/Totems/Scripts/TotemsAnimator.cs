using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MiniGames.Modules.Level.Utils;
using UnityEngine.UI;
using System;

namespace MiniGames.Modules.Level.Totems
{
    public class TotemsAnimator : MonoBehaviour
    {
        [SerializeField] private List<GameObject> checkMarks;
        [Header("Particles")]
        [Space]
        [SerializeField] private ParticleSystem exampleWigwamParticle;
        [SerializeField] private ParticleSystem playerWigwamParticle;
        [SerializeField] private List<ParticleSystem> totemsParticles;
        [Header("ExamplePart")]
        [Space]
        [SerializeField] private Transform exampleWigwam;
        [SerializeField] private Transform exampleTotemRevealPoint;
        [SerializeField] private List<Transform> exampleTotemEndPoints;
        [SerializeField] private List<Image> exampleTotemParts;
        [Header("PlayerPart")]
        [Space]
        [SerializeField] private Transform playerTotemRevealPoint;
        [SerializeField] private List<Image> playerTotemParts;
        [SerializeField] private Transform playerWigwam;
        [SerializeField] private List<DropZone> answerFields;
        private Vector3 exampleWigwamScale;
        private Dictionary<Transform, Vector3> exampleTotemPartsScales;
        private Dictionary<Transform, Vector3> playerTotemPartsScales;
        private Vector3 playerWigwamScale;
        private int count;

        private void Awake()
        {
            exampleTotemPartsScales = new();
            playerTotemPartsScales = new();
            exampleWigwamScale = exampleWigwam.localScale;
            playerWigwamScale = playerWigwam.localScale;
        }
     
        private void Start()
        {
            for (int i = 0; i < answerFields.Count; i++)
            {
                exampleTotemPartsScales[exampleTotemParts[i].transform] = exampleTotemParts[i].transform.localScale;
                exampleTotemParts[i].gameObject.SetActive(false);

                playerTotemPartsScales[playerTotemParts[i].transform] = playerTotemParts[i].transform.localScale;
                playerTotemParts[i].gameObject.SetActive(false);

                answerFields[i].gameObject.SetActive(false);
            }
            foreach (var item in checkMarks)
            {
                item.SetActive(false);
            }
        }

        public void ShowAnimation(List<Sprite> correctTotems,Action onComplete)
        {
            foreach (var item in exampleTotemParts)
            {
                item.transform.position = exampleTotemRevealPoint.position;
                item.transform.localScale = exampleTotemPartsScales[item.transform];
            }
            foreach (var item in playerTotemParts)
            {
                item.transform.position = playerTotemRevealPoint.position;
                item.transform.localScale = playerTotemPartsScales[item.transform];
            }
            count = 0;
            for (int i = 0; i < exampleTotemParts.Count; i++)
            {
                exampleTotemParts[i].sprite = correctTotems[i];
            }
            ExampleWigwamAnimation(onComplete);
        }

        public void PlayerWigwamAnimation(Action onComplete)
        {
            Sequence seq = DOTween.Sequence(transform);
            seq.Append(playerWigwam.DOMove(playerWigwam.position + playerWigwam.up * 1.5f, 0.6f).SetEase(Ease.OutQuad))
                .Join(playerWigwam.DOScaleX(playerWigwamScale.x * 0.97f, 0.4f))
                .Append(playerWigwam.DOMove(playerWigwam.position, 0.2f).OnComplete(() =>
                {
                    playerWigwamParticle.Play();
                    onComplete?.Invoke();
                }))
                .Join(playerWigwam.DOScaleX(playerWigwamScale.x * 1.05f, 0.3f))
                .Append(playerWigwam.DOScaleX(playerWigwamScale.x, 0.3f));
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

        public void CorrectTaskAnswerAnimation(Action onComplete)
        {
            Sequence seq = DOTween.Sequence(transform);
            seq.Pause();
            for (int i = 0; i < answerFields.Count; i++)
            {
                seq.Join(exampleTotemParts[i].transform.DOMove(
                    exampleTotemParts[i].transform.position + exampleTotemParts[i].transform.up * 1.2f, 0.3f));                  
                seq.Join(playerTotemParts[i].transform.DOMove(
                    playerTotemParts[i].transform.position + playerTotemParts[i].transform.up * 1.2f, 0.3f));
            }
            seq.SetEase(Ease.Linear);
            seq.SetLoops(2, LoopType.Yoyo);
            seq.OnComplete(() =>
            {
                foreach (var item in totemsParticles)
                {
                    item.Play();
                }
                onComplete?.Invoke();
            });
            seq.Play();
            
        }

        public void HideAnimation(Action onComplete)
        {
            Sequence seq = DOTween.Sequence(transform);
            seq.Pause();
            foreach (var item in exampleTotemParts)
            {
                seq.Join(item.transform.DOScale(Vector3.zero, 0.2f));
            }
            foreach (var item in playerTotemParts)
            {
                seq.Join(item.transform.DOScale(Vector3.zero, 0.2f));
            }
            seq.SetEase(Ease.InOutFlash, -1);
            seq.OnComplete(() => 
            {
                foreach (var item in exampleTotemParts)
                {
                    item.gameObject.SetActive(false);
                }
                onComplete?.Invoke(); 
            });
            seq.Play();
        }

        private void ExampleWigwamAnimation(Action onComplete)
        {
            Sequence seq = DOTween.Sequence(transform);
            seq.Append(exampleWigwam.DOMove(exampleWigwam.position + exampleWigwam.up * 1.5f, 0.6f).SetEase(Ease.OutQuad))
                .Join(exampleWigwam.DOScaleX(exampleWigwamScale.x * 0.97f, 0.4f))
                .Append(exampleWigwam.DOMove(exampleWigwam.position, 0.2f).OnComplete(()=>
                {
                    exampleWigwamParticle.Play();
                    ShowExampleTotemPart();
                }))
                .Join(exampleWigwam.DOScaleX(exampleWigwamScale.x*1.05f, 0.3f))
                .Append(exampleWigwam.DOScaleX(exampleWigwamScale.x, 0.3f));
            seq.SetLoops(3, LoopType.Restart);
            seq.OnComplete(() => 
            {
                count = 0;
                AnswerFieldsAnimation(onComplete);
            });
        }

        private void AnswerFieldsAnimation(Action onComplete)
        {
            Sequence seq = DOTween.Sequence(transform);
            seq.Pause();
            for (int i = answerFields.Count-1; i >= 0; i--)
            {
                var defScale = answerFields[i].transform.localScale;
                answerFields[i].transform.localScale = Vector3.zero;
                answerFields[i].gameObject.SetActive(true);
                seq.Join(answerFields[i].transform.DOScale(defScale, 0.35f));
            }
            seq.SetEase(Ease.InBounce,-1);
            seq.SetDelay(0.4f); 
            seq.OnComplete(() => onComplete?.Invoke());
            seq.Play();
        }

        private void ShowExampleTotemPart()
        {
            exampleTotemParts[count].gameObject.SetActive(true);
            exampleTotemParts[count].transform.DOJump(exampleTotemEndPoints[count].position, 2f, 1, 0.7f);
            count++;
        }

        private void OnDestroy()
        {
            transform.DOKill();
            foreach (var item in exampleTotemParts)
            {
                item.transform.DOKill();
            }
        }
    }
}
