using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace MiniGames.Modules.Level.Utils
{
    public class BackToMenuSlider : MonoBehaviour, IEndDragHandler, IDragHandler
    {
        public event Action backToMenuEvent;

        [SerializeField] private Slider slider;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            slider.interactable = true;
        }
        private void OnDisable()
        {
            slider.interactable = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (slider.value == slider.maxValue)
            {
                slider.interactable = false;
                backToMenuEvent?.Invoke();
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (slider.value !=slider.maxValue)
            {
                slider.value = slider.minValue;
            }
        }
    }
}

