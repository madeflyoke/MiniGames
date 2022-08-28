using UnityEngine;
using UnityEngine.UI;

namespace MiniGames.Modules.Level.MatchTwo
{
    public class CloudsMover : MonoBehaviour
    {
        [SerializeField] private Image clouds;
        [SerializeField] private float speed;

        private Material mat;

        private void Awake()
        {
            mat = clouds.material;
        }

        private void Update()
        {
            mat.mainTextureOffset += Vector2.left * speed * Time.deltaTime;
        }

        private void OnDisable()
        {
            mat.mainTextureOffset = Vector2.zero;
        }
    }
}
