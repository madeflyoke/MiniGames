using System.Collections;
using System.Collections.Generic;
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
            rectTransform.DOKill();
            rectTransform.DOScale(rectTransform.localScale / enterScale, 0.3f).endValue = defaultScale;
            
            if (eventData.pointerDrag == correctObject.gameObject/*&&Draggable.s_currentDraggable==eventData.pointerDrag*/)
            {
                correctObject.selfControl = false;
                correctObject.transform.DOKill();
                correctObject.transform.DOMove(transform.position, 0.2f).OnStart(() =>
                {
                    correctAnswerEvent?.Invoke();
                    gameObject.SetActive(false);
                });

            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null /*&& Draggable.s_currentDraggable == eventData.pointerDrag*/)
            {
                rectTransform.DOKill();
                rectTransform.DOScale(rectTransform.localScale * enterScale, 0.2f).startValue = defaultScale;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null /*&& Draggable.s_currentDraggable == eventData.pointerDrag*/)
            {
                rectTransform.DOKill();
                rectTransform.DOScale(rectTransform.localScale / enterScale, 0.2f).endValue = defaultScale;
            }
        }

    }

}
