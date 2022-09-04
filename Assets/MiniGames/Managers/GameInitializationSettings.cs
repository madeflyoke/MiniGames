using UnityEngine;

namespace MiniGames.Managers
{
    public class GameInitializationSettings : MonoBehaviour
    {
        [SerializeField] private int fps;

        private void Awake()
        {
            Application.targetFrameRate = fps;
            Input.multiTouchEnabled = false;
        }

    }
}

