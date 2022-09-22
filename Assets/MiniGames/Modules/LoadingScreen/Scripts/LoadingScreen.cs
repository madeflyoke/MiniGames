using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace MiniGames.Modules.LoadingScreen
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer movingPart;
        [SerializeField] private SpriteRenderer sky;
        [SerializeField] private SpriteRenderer text;

        [SerializeField] private float speed;
        [SerializeField] private float amplitude;
        [SerializeField] private float frequency;
        private Material mat;
        private CancellationTokenSource cts;

        private void Awake()
        {
            cts = new CancellationTokenSource();
            mat = movingPart.material;
        }

        public async void StartAnimation()
        {
            gameObject.SetActive(true);
            while (true)
            {
                mat.mainTextureOffset += Vector2.left * speed * Time.deltaTime;
                mat.mainTextureOffset = new Vector2(mat.mainTextureOffset.x, Mathf.Sin(Time.time * frequency) * amplitude);
                await UniTask.Yield(cts.Token);
            }
        }         

        public void StopAnimation()
        {
            if (cts !=null)
            {
               cts.Cancel();
            }
            cts = new CancellationTokenSource();
            mat.mainTextureOffset = Vector3.zero;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (cts != null)
            {
                cts.Cancel();
            }
        }
    }
}

