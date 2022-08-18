using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace MiniGames.Modules.LoadingScreen
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer movingPart;
        [SerializeField] private float speed;
        [SerializeField] private float amplitude;
        [SerializeField] private float frequency;
        private Material mat;
        private CancellationTokenSource cancellationToken;

        private void Awake()
        {
            cancellationToken = new CancellationTokenSource();
            mat = movingPart.material;
            gameObject.SetActive(false);
        }

        public async void StartAnimation()
        {
            gameObject.SetActive(true);
            while (true)
            {
                mat.mainTextureOffset += Vector2.right * speed * Time.deltaTime;
                mat.mainTextureOffset = new Vector2(mat.mainTextureOffset.x, Mathf.Sin(Time.time * frequency) * amplitude);
                await UniTask.Yield(cancellationToken.Token);
            }
        }         

        public void StopAnimation()
        {
            cancellationToken.Cancel();
            cancellationToken = new CancellationTokenSource();
            mat.mainTextureOffset = Vector2.zero;
            gameObject.SetActive(false);
        }
    }
}

