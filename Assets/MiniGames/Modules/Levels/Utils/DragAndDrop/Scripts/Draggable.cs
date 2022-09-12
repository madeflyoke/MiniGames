using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace MiniGames.Modules.Level.Utils
{
    [RequireComponent(typeof(Image), typeof(RectTransform))]
    public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler
    {
        [SerializeField] private float returnBackTime = 0.7f;
        [HideInInspector]
        public bool selfControl;
        public Image Image { get; private set; }
        public Vector3 DefaultPos { get; set; }
        private RectTransform rectTransform;
        private Canvas canvas;

        private void Awake()
        {
            selfControl = true;
            Image = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            DefaultPos = transform.position; //world pos
        }


        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (selfControl)
            {
                rectTransform.DOKill();
                transform.DOMove(DefaultPos, returnBackTime).OnComplete(
                    () => { Image.raycastTarget = true;});              
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            rectTransform.DOKill();
            Image.raycastTarget = false;
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;         
        }

        public void ResetValues()
        {
            transform.position = DefaultPos;
            Image.raycastTarget = true;
            selfControl = true;
        }
    }
}

