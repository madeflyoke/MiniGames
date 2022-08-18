using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace MiniGames.Modules.Main.Menu
{
    public class MainMenuController : MonoBehaviour
    {
        public event Action transitionOutCompleteEvent;
        public event Action transitionInCompleteEvent;

        [SerializeField] private Button playButton;
        [Header("Transition/Zoom")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Transform zoomPivot;
        [SerializeField] private float endZoomValue;
        [SerializeField] private float zoomDuration;
        [Header("Transition/Fading")]
        [SerializeField] private float zoomValueTriggerToFade;
        [SerializeField] private float fadeDuration;
        [Header("Clouds")]
        [SerializeField] private Image skyClouds;
        [SerializeField] private float skySpeed;
        [Header("Water")]
        [SerializeField] private Transform waterPivot;
        [SerializeField] private float frequency;
        [SerializeField] private float amplitude;
        [SerializeField] private ParticleSystem waterVfx;
        private Material skyMat;
        private CancellationTokenSource cancellationToken;

        void Start()
        {
            skyMat = skyClouds.material;
            waterVfx.gameObject.SetActive(true);
            waterVfx.Play();
            cancellationToken = new CancellationTokenSource();
        }

        void Update()
        {
            skyMat.mainTextureOffset += new Vector2(-Time.deltaTime * skySpeed, 0);
            waterPivot.localScale += new Vector3(0f, amplitude * Mathf.Sin(Time.time * frequency), 0f);
        }

        private async void StartZoomOut()
        {
            playButton.gameObject.SetActive(false);
            waterVfx.Stop();
            await UniTask.Delay(250,cancellationToken:cancellationToken.Token);
            transform.DOScale(endZoomValue, zoomDuration).SetEase(Ease.InOutExpo);
            await UniTask.WaitUntil(() => transform.localScale.x >= zoomValueTriggerToFade, cancellationToken: cancellationToken.Token);
            canvasGroup.DOFade(0f, fadeDuration);
            await UniTask.WaitUntil(() => canvasGroup.alpha == 0 && transform.localScale.x == endZoomValue, cancellationToken: cancellationToken.Token);
            transitionOutCompleteEvent?.Invoke();
            gameObject.SetActive(false);
        }

        private async void StartZoomIn()
        {
            await UniTask.Delay(250, cancellationToken: cancellationToken.Token);
            gameObject.SetActive(true);
            transform.DOScale(1f, zoomDuration).SetEase(Ease.InOutExpo);
            await UniTask.WaitUntil(() => transform.localScale.x >= zoomValueTriggerToFade, cancellationToken: cancellationToken.Token);
            canvasGroup.DOFade(1f, fadeDuration);
            await UniTask.WaitUntil(() => canvasGroup.alpha == 1 && transform.localScale.x == 1f, cancellationToken: cancellationToken.Token);
            waterVfx.Play();
            transitionInCompleteEvent?.Invoke();
            playButton.gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            cancellationToken = new CancellationTokenSource();
            playButton.interactable = true;
            playButton.onClick.AddListener(() =>
            {
                playButton.interactable = false;
                playButton.transform.DOPunchScale(Vector3.one*0.1f,0.1f).OnComplete(()=>
                playButton.transform.DOScale(0, 0.05f).OnComplete(() => StartZoomOut()));
            });
        }
        private void OnDisable()
        {
            cancellationToken.Cancel();
            playButton.onClick.RemoveAllListeners();
            skyMat.mainTextureOffset = Vector2.zero;
        }
    }

}
