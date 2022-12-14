using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using DG.Tweening;
using UnityEngine.UI;
using Zenject;

namespace MiniGames.Modules.Level.Utils
{
    public class Scratcher : MonoBehaviour
    {
        [Inject] private SpritesOriginalSizes spritesOriginalSizes;

        private const string rtTexture = "_DrawTexture";

        public event Action startTouchingEvent;
        public event Action exitButtonPressedEvent;

        [Header("Utils")]
        [SerializeField] private ParticleSystem scratchEffect;
        [SerializeField] private ParticleSystem godRaysEffect;
        [SerializeField] private GameObject helperPointer;
        [SerializeField] private SpriteRenderer background;
        [SerializeField] private Button exitButton;

        [Header("Scratching")]
        [SerializeField] private GameObject trailPrefab;
        [SerializeField] private SpriteRenderer targetSr;
        [SerializeField] private Camera renderTextureCam;
        [SerializeField] private float brushSize;

        private ScratcherProgress scratcherProgress;

        private Camera mainCam;
        private Vector2 correctionOffset;
        private RenderTexture currentRT;
        private CancellationTokenSource cts;
        private bool canScratch;

        private void Awake()
        {
            currentRT = null;
            renderTextureCam.targetTexture=null;
            RenderTexture.active = null;
            exitButton.transform.parent.gameObject.SetActive(false);
            mainCam = Camera.main;
            renderTextureCam.transform.position = new Vector3(0, 0, trailPrefab.transform.position.z - 5);
            cts = new CancellationTokenSource();
            trailPrefab.GetComponent<TrailRenderer>().widthMultiplier = brushSize;
            helperPointer.SetActive(false);
            scratcherProgress = GetComponent<ScratcherProgress>();
            Initialize();
            gameObject.SetActive(false); //last
        }

        private void Initialize()
        {
            Sprite sprite = targetSr.sprite;
            Vector2 worldMainTextureCenter = new Vector2(sprite.texture.width / 2, sprite.texture.height / 2) / 100;
            Vector2 worldSpriteCenter = sprite.rect.center / 100;
            Vector2 size = spritesOriginalSizes.GetOriginalWidthHeight(sprite.texture);
            int width = (int)size.x;
            int height = (int)size.y;
            Vector2 sizeCorrections = new Vector2((float)width / sprite.texture.width, (float)height / sprite.texture.height);
            correctionOffset = worldSpriteCenter - worldMainTextureCenter;
            correctionOffset = new Vector2(correctionOffset.x, correctionOffset.y) * sizeCorrections.x * targetSr.transform.localScale;
          
            currentRT = RenderTexture.GetTemporary(sprite.texture.width, sprite.texture.height, 1);
            RenderTexture.active = currentRT;
            renderTextureCam.targetTexture = currentRT;
            renderTextureCam.orthographicSize = sprite.texture.height / sprite.pixelsPerUnit
                * targetSr.transform.localScale.x / 2;
            renderTextureCam.Render();

            targetSr.material.SetTexture(rtTexture, currentRT);
            targetSr.material.SetFloat("_isActive", 1f);
            scratcherProgress.Initialize(sprite, currentRT);
        }

        public void StartScratching()
        {
            ClearCameraBuffer();
            Color tmpColor = background.color;
            background.color = new Color(background.color.r, background.color.g,
                background.color.b, 0f);
            Vector3 tmpScale = transform.localScale;
            transform.localScale = Vector3.one / 2;
            gameObject.SetActive(true);
            background.DOColor(tmpColor, 0.3f);
            transform.DOScale(tmpScale, 0.3f).OnComplete(() =>
            {
                canScratch = true;
                godRaysEffect.Play();
                var tmpscale = helperPointer.transform.localScale;
                helperPointer.transform.localScale = Vector3.zero;
                helperPointer.SetActive(true);

                Sequence s = DOTween.Sequence();
                s
                .Append(helperPointer.transform.DOScale(tmpscale, 0.5f))
                .Append(helperPointer.transform.DORotate(new Vector3(0, 0, -25f), 1f))
                .Append(helperPointer.transform.DORotate(new Vector3(0, 0, 25f), 1f))
                .Append(helperPointer.transform.DORotate(new Vector3(0, 0, -25f), 1f))
                .Append(helperPointer.transform.DORotate(new Vector3(0, 0, 25f), 1f))
                .Append(helperPointer.transform.DOScale(Vector3.zero, 0.3f))
                .OnComplete(() => helperPointer.gameObject.SetActive(false))
                .SetEase(Ease.Linear).SetDelay(1f);
            });
        }

        private void EndScratching()
        {
            canScratch = false;
            targetSr.material.SetFloat("_isActive", 0f);
            targetSr.transform.DOScale(targetSr.transform.localScale * 1.05f, 2.5f).SetEase(Ease.OutCirc)
                .OnComplete(() =>
                {
                    exitButton.transform.parent.gameObject.SetActive(true);
                    exitButton.onClick.AddListener(() => {
                        exitButton.transform.DOPunchScale(Vector3.one * 0.1f,0.2f).OnComplete(()=>
                         exitButtonPressedEvent?.Invoke());                   
                    }); 
                });
        }

        private void ClearCameraBuffer()
        {
            renderTextureCam.clearFlags = CameraClearFlags.Color;
            renderTextureCam.backgroundColor = new Color(0, 0, 0, 0);
            renderTextureCam.Render();
            renderTextureCam.clearFlags = CameraClearFlags.Nothing;
            renderTextureCam.Render();
        }

        private void OnEnable()
        {
            scratcherProgress.fillingCompleteEvent += EndScratching;
        }
        private void OnDisable()
        {
            scratcherProgress.fillingCompleteEvent -= EndScratching;
        }

        private void OnDestroy()
        {
            if (targetSr!=null&&rtTexture!=null)
                targetSr.material.SetTexture(rtTexture, null);
            if (cts!=null)
                 cts.Cancel();
            currentRT.Release();
            currentRT.DiscardContents();
        }

        void Update()
        {
            if (canScratch)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    SetTrail();
                    startTouchingEvent?.Invoke();
                }
                else if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    cts.Cancel();
                }
            }
        }
       
        private async void SetTrail()
        {
            if (helperPointer.activeInHierarchy)
            {
                helperPointer.transform.DOKill();
                helperPointer.SetActive(false);
            }
            cts = new CancellationTokenSource();
            Vector3 pos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 posOffseted = pos +(Vector3)correctionOffset;
            Vector3 correctMousePos = new Vector3(posOffseted.x, posOffseted.y, 0f);
            scratchEffect.gameObject.SetActive(false);
            scratchEffect.transform.position = new Vector3(pos.x, pos.y, mainCam.transform.position.z + 10f);
            scratchEffect.gameObject.SetActive(true);
            scratchEffect.Play();

            var trail = Instantiate(trailPrefab, correctMousePos, Quaternion.identity,transform);
            while (Input.GetKey(KeyCode.Mouse0))
            {
                pos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                posOffseted = pos + (Vector3)correctionOffset;
                correctMousePos = new Vector3(posOffseted.x, posOffseted.y, 0f);
                scratchEffect.transform.position = new Vector3(pos.x, pos.y, mainCam.transform.position.z + 10f);
                trail.transform.position = correctMousePos;
                await UniTask.Yield(cts.Token);
            }
        }
    }
}

