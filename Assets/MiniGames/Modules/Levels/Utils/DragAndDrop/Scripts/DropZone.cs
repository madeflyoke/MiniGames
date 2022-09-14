using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

namespace MiniGames.Modules.Level.Utils
{
    [RequireComponent(typeof(RectTransform))]
    public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action correctAnswerEvent;

        public enum DropAction
        {
            Disappear,
            TakeIn
        }

        [SerializeField] private DropAction dropAction;
        [SerializeField] private float enterScale;
        private RectTransform rectTransform;
        private Vector3 defaultScale;
        private Draggable correctObject;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            defaultScale = rectTransform.localScale;
        }

        public void Initialize(Draggable correct)
        {
            if (correctObject!=null)
            {
                correctObject.ResetValues();
            }
            correctObject = correct;
        }

        public void OnDrop(PointerEventData eventData)
        {                
            if (eventData.pointerDrag == correctObject.gameObject)
            {
                correctObject.selfControl = false;
                correctObject.transform.DOKill();
                switch (dropAction)
                {                       
                    case DropAction.Disappear:
                        rectTransform.DOKill();
                        rectTransform.DOScale(rectTransform.localScale / enterScale, 0.3f).endValue = defaultScale;
                        correctObject.transform.DOMove(transform.position, 0.2f).OnStart(() =>
                        {
                            gameObject.SetActive(false);
                            correctAnswerEvent?.Invoke();
                        });
                        break;
                    case DropAction.TakeIn:
                        DOTween.Sequence()
                            .Append(correctObject.transform.DOMove(transform.position, 0.15f))
                            .Append(correctObject.transform.DOScale(correctObject.transform.localScale * 1.2f, 0.3f))
                            .Append(correctObject.transform.DOScale(Vector3.zero, 0.15f))
                        .OnComplete(() =>
                        {
                            correctObject.gameObject.SetActive(false);
                            correctObject.ResetValues();
                            rectTransform.DOKill();
                            rectTransform.DOScale(rectTransform.localScale / enterScale, 0.2f).endValue = defaultScale;
                            correctAnswerEvent?.Invoke();
                        }).SetEase(Ease.OutQuad, -1);
                        break;
                }
            }
            else
            {
                rectTransform.DOKill();
                rectTransform.DOScale(rectTransform.localScale / enterScale, 0.3f).endValue = defaultScale;
            }
                
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                rectTransform.DOKill();
                rectTransform.DOScale(rectTransform.localScale * enterScale, 0.2f).startValue = defaultScale;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {          
            if (eventData.pointerDrag != null)
            {
                rectTransform.DOKill();
                rectTransform.DOScale(rectTransform.localScale / enterScale, 0.2f).endValue = defaultScale;
            }
        }

    }

}
