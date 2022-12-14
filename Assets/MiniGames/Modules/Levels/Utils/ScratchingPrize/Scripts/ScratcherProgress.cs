using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.Collections;

namespace MiniGames.Modules.Level.Utils
{
    public class ScratcherProgress : MonoBehaviour
    {
        public event Action fillingCompleteEvent;

        [BurstCompile]
        private struct PixelJob : IJob
        {
            public Rect spriteRectOnScreen;
            public int targetPixelsCount;
            public NativeArray<Color> spritePixels;
            public NativeArray<Color32> rtPixels;
            public NativeArray<int> result;

            public void Execute()
            {
                int targetPixelsFilled = 0;
                for (int i = 0; i < rtPixels.Length; i++)
                {
                    if (spritePixels[i].a != 0 && rtPixels[i].a != 0)
                    {
                        targetPixelsFilled++;
                    }
                }
                int percentage = targetPixelsFilled * 100 / targetPixelsCount; //if need percentage??  
                result[1] = targetPixelsCount - targetPixelsFilled;
                result[0] = percentage;         
            }
        }

        [Range(0f,100f)]
        [Tooltip("Complicated images can provide hard-seeing areas, " +
            "specifically in same color with shader texture/color, in percents, default value = 1.5f")]
        [SerializeField] private float currentImageFilnessErorr = 0.015f;

        private Scratcher scratcher;
        private RenderTexture currentRT;
        private Color[] spritePixels;
        private Rect spriteRectOnScreen;
        private Texture2D spriteCopy;
        private int targetPixelsCount;
        private CancellationTokenSource cts;

        private void Awake()
        {
            scratcher = GetComponent<Scratcher>();
        }

        public void Initialize(Sprite sprite, RenderTexture renderTexture)
        {
            spritePixels = sprite.texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y,
               (int)sprite.rect.width, (int)sprite.rect.height);
         
            currentRT = renderTexture;
            spriteCopy = new Texture2D((int)sprite.rect.width,
                (int)sprite.rect.height, TextureFormat.RGBA32, false);

            spriteRectOnScreen = new Rect((int)sprite.rect.xMin,
              (sprite.texture.height - (int)sprite.rect.yMax),
            (int)sprite.rect.width,
            (int)sprite.rect.height);

            targetPixelsCount = 0;
            for (int i = 0; i < spritePixels.Length; i++)
            {
                if (spritePixels[i].a != 0)
                    targetPixelsCount++;
            }
        }

        private void OnEnable()
        {
            scratcher.startTouchingEvent += CheckProgress;
        }
        private void OnDisable()
        {
            scratcher.startTouchingEvent -= CheckProgress;
        }


        [BurstCompile]
        private async void CheckProgress()
        {
            cts = new CancellationTokenSource();
            while (Input.GetKey(KeyCode.Mouse0)) 
            {
                RenderTexture.active = currentRT; //read pixels on late update? works by now 
                spriteCopy.ReadPixels(spriteRectOnScreen, 0, 0);
                NativeArray<Color32> rtPixels = spriteCopy.GetRawTextureData<Color32>();               
                NativeArray<Color> targetSpritePixels = new NativeArray<Color>(spritePixels, Allocator.TempJob);
                NativeArray<int> resultArray = new NativeArray<int>(2, Allocator.TempJob);

                PixelJob job = new PixelJob
                {
                    rtPixels = rtPixels,
                    spritePixels = targetSpritePixels, 
                    spriteRectOnScreen = this.spriteRectOnScreen,
                    targetPixelsCount = this.targetPixelsCount,
                    result = resultArray              
                };
                JobHandle handler = job.Schedule();
                handler.Complete();

                if (resultArray[1]<=spritePixels.Length*currentImageFilnessErorr/100)
                {
                    fillingCompleteEvent?.Invoke();
                    targetSpritePixels.Dispose();
                    resultArray.Dispose();
                    rtPixels.Dispose();
                    return;
                }

                //if (targetPixelsCount == targetPixelsFilled)
                //{
                //    Debug.Log("100");
                //}
                targetSpritePixels.Dispose();
                resultArray.Dispose();
                rtPixels.Dispose();
                await UniTask.Delay(1000, cancellationToken:cts.Token); //set delay to each loop of checking        
            }
        }


    }

}
