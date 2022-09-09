using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace MiniGames.Modules.Level.Utils
{
    public class MatchTwoHelper : TutorialHelper
    {
        public void Initialize(Func<bool> stopTrigger, List<Image> pivots)
        {
            base.Initialize(stopTrigger);
            var firstPivot = pivots[UnityEngine.Random.Range(0, pivots.Count)];
            helperPointers[0].transform.position = firstPivot.transform.position;
            foreach (var secondPivot in pivots)
            {
                if (secondPivot.sprite == firstPivot.sprite && secondPivot != firstPivot)
                {
                    helperPointers[1].transform.position = secondPivot.transform.position;
                    break;
                }
            }
        }
    }
}

