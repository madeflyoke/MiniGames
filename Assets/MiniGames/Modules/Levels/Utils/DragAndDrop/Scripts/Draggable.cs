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
                transform.DOMove(DefaultPos, returnBackTime).OnComplete(
                    () => { Image.raycastTarget = true; s_currentDraggable = null; });              
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (s_currentDraggable == null)
            {
                rectTransform.DOKill();
                s_currentDraggable = eventData.pointerDrag;
                Image.raycastTarget = false;
            }
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;         
        }

        public void ResetValues()
        {
            transform.position = DefaultPos;
            Image.raycastTarget = true;
            s_currentDraggable = null;
            selfControl = true;
        }

        private void OnDestroy()
        {
            s_currentDraggable = null;
        }
    }
}

