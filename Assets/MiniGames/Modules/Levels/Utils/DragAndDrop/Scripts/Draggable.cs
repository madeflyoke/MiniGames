using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace MiniGames.Modules.Level.Utils
{
    [RequireComponent(typeof(Image), typeof(RectTransform))]
    public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler
    {
        public static GameObject s_currentDraggable;

        [SerializeField] private float returnBackTime = 0.7f;
        [HideInInspector]
        public bool selfControl;

        private Image image;
        private RectTransform rectTransform;
        private Canvas canvas;
        private Vector2 defaultPos;

        private void Awake()
        {
            selfControl = true;
            image = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            defaultPos = rectTransform.anchoredPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (s_currentDraggable == eventData.pointerDrag)
            {
                rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            }
           
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (selfControl && s_currentDraggable == eventData.pointerDrag)
            {
                rectTransform.DOKill();
                rectTransform.DOAnchorPos(defaultPos, returnBackTime).OnComplete(
                    () => { image.raycastTarget = true; s_currentDraggable = null; });              
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (s_currentDraggable == null)
            {
                s_currentDraggable = eventData.pointerDrag;
                image.raycastTarget = false;
            }
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;         
        }

        public void ResetValues()
        {
            rectTransform.anchoredPosition = defaultPos;
            image.raycastTarget = true;
            s_currentDraggable = null;
            selfControl = true;
        }

        private void OnDestroy()
        {
            s_currentDraggable = null;
        }
    }
}

