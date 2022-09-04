using UnityEngine;

namespace MiniGames.Modules.Main.Menu.ChooseMenu
{
    public class HousesIsland : Island
    {
        [Header("Animation")]
        [SerializeField] private Transform rotationPivot;
        [SerializeField] private float speed;
        [SerializeField] private float angle;
        private bool canAnimate;

        public override void StartAnimation()
        {
            canAnimate = true;
        }

        public override void StopAnimation()
        {
            canAnimate = false;
        }

        private void Update()
        {
            if (canAnimate)
            {
                rotationPivot.rotation = Quaternion.Euler(rotationPivot.rotation.x,
                    rotationPivot.rotation.y,Mathf.Sin(speed*Time.time)*angle);
            }
        }
    }
}

