using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGames.Modules.Level.Utils
{
    public class SimpleRotator : MonoBehaviour
    {
        [SerializeField] private float speed;

        private void Update()
        {
            transform.rotation *= Quaternion.Euler(0, 0, Time.deltaTime * speed);
        }
    }
}

