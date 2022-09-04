using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGames.Modules.Level.Utils
{
    [RequireComponent(typeof(Button))]
    public class ScratchAgainButton : MonoBehaviour
    {
        [SerializeField] private Scratcher scratcher;
        public Button Button => button;
        private Button button;
        private Vector3 defaultScale;

        private void Awake()
        {
            defaultScale = transform.localScale;
            button = GetComponent<Button>();
            gameObject.SetActive(false);
        }

        public void Activate()
        {           
            transform.DOScale(defaultScale, 0.2f).ChangeStartValue(Vector3.zero).SetEase(Ease.Linear, -1);
            gameObject.SetActive(true);
            button.interactable = true;
        }

        private void OnEnable()
        {
            button.onClick.AddListener(ButtonListener);
        }
        private void OnDisable()
        {
            button.onClick.RemoveListener(ButtonListener);
        }

        private void ButtonListener()
        {
            button.interactable = false;
            transform.DOKill();
            transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 5).OnComplete(() =>
            {
                scratcher.StartScratching();
            });
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }

    }
}

