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
        private Vector3 defaultPos;
        private RectTransform rectTransform;
        private Canvas canvas;
        private Vector3 defaultScale;
        private bool customDefaultPosition;

        private void Awake()
        {
            selfControl = true;
            Image = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            defaultScale = transform.localScale;
        }

        private void Start()
        {
            if (customDefaultPosition==false)
            {
                defaultPos = transform.position;
            }
        }

        public void SetDefaultPosition(Vector3 pos) //so manual control
        {
            customDefaultPosition = true;
            defaultPos = pos;
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
                transform.DOMove(defaultPos, returnBackTime).OnComplete(
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
            gameObject.SetActive(false);
            transform.position = defaultPos;
            transform.localScale = defaultScale;
            Image.raycastTarget = true;
            selfControl = true;
        }
    }
}

