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
        private bool canAnimate;
        

        void Awake()
        {
            skyMat = skyClouds.material;
            cancellationToken = new CancellationTokenSource();
        }

        public void Initialize(MenuModule.Mode mode)
        {
            switch (mode)
            {
                case MenuModule.Mode.MainMenu:
                    ZoomInPreparation();
                    break;
                case MenuModule.Mode.ChooseMenu:
                    ZoomOutPreparation();
                    break;
                default:
                    break;
            }
        }

        void Update()
        {
            if (canAnimate)
            {
                skyMat.mainTextureOffset += new Vector2(-Time.deltaTime * skySpeed, 0);
                waterPivot.localScale += new Vector3(0f, amplitude * Mathf.Sin(Time.time * frequency), 0f);
            }           
        }

        private async void StartZoomIn()
        {
            playButton.gameObject.SetActive(false);
            waterVfx.Stop();
            waterVfx.gameObject.SetActive(false);
            await UniTask.Delay(250,cancellationToken:cancellationToken.Token);
            transform.DOScale(endZoomValue, zoomDuration).SetEase(Ease.InOutExpo);
            await UniTask.WaitUntil(() => transform.localScale.x >= zoomValueTriggerToFade, cancellationToken: cancellationToken.Token);
            canvasGroup.DOFade(0f, fadeDuration);
            await UniTask.WaitUntil(() => canvasGroup.alpha == 0 && transform.localScale.x == endZoomValue, cancellationToken: cancellationToken.Token);
            transitionOutCompleteEvent?.Invoke();
            gameObject.SetActive(false);
        }

        public async void StartZoomOut()
        {          
            zoomPivot.gameObject.SetActive(true);
            gameObject.SetActive(true);
            canAnimate = true;
            transform.DOScale(1f, zoomDuration*0.9f).SetEase(Ease.OutExpo);
            await UniTask.WaitUntil(() => transform.localScale.x <= endZoomValue-zoomValueTriggerToFade, cancellationToken: cancellationToken.Token);
            canvasGroup.DOFade(1f, fadeDuration*0.9f);
            await UniTask.WaitUntil(() => canvasGroup.alpha == 1 && transform.localScale.x == 1f, cancellationToken: cancellationToken.Token);
            waterVfx.gameObject.SetActive(true);
            waterVfx.Play();
            transitionInCompleteEvent?.Invoke();
            playButton.transform.localScale = Vector3.one;
            playButton.gameObject.SetActive(true);
        }

        private void ZoomInPreparation()
        {
            zoomPivot.gameObject.SetActive(true);
            playButton.gameObject.SetActive(true);
            gameObject.SetActive(true);
            waterVfx.gameObject.SetActive(true);
            waterVfx.Play();
            canAnimate = true;
        }

        private void ZoomOutPreparation()
        {
            gameObject.SetActive(false);
            canvasGroup.alpha = 0f;
            transform.localScale = Vector3.one * endZoomValue;
            zoomPivot.gameObject.SetActive(false);
            playButton.gameObject.SetActive(false);
            waterVfx.gameObject.SetActive(false);
            waterVfx.Stop();
            canAnimate = false;
        }

        private void OnEnable()
        {
            playButton.interactable = true;
            playButton.onClick.AddListener(() =>
            {
                playButton.interactable = false;
                playButton.transform.DOPunchScale(Vector3.one*0.1f,0.2f).OnComplete(()=>
                playButton.transform.DOScale(0, 0.1f).OnComplete(() => StartZoomIn()));
            });
        }
        private void OnDisable()
        {
            cancellationToken.Cancel();
            cancellationToken = new CancellationTokenSource();
            playButton.onClick.RemoveAllListeners();
            skyMat.mainTextureOffset = Vector2.zero;
        }
    }

}
