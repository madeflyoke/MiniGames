using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using MiniGames.Extensions;
using MiniGames.Modules.Level.Utils;

namespace MiniGames.Modules.Level.ColorBuckets
{
    public class ColorBucketsController : MonoBehaviour
    {
        private const int _levelsCount = 5;

        [SerializeField] private List<Image> buckets;
        [SerializeField] private List<Image> toysPivots;
        [SerializeField] private List<Color> mainColors;
        [SerializeField] private List<Sprite> toys;
        private List<List<Color>> previousColorSets;

        private void Awake()
        {
            previousColorSets = new();
            toys = toys.Shuffle();
        }

        private void Start()
        {
            SetAnswers();

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                SetAnswers();
            }
        }

        private async void SetAnswers()
        {
            SetToys();
            List<Color> currentColors=new(mainColors);
            List<Color> finalColors;
            do
            {
                currentColors = currentColors.Shuffle();
                finalColors = CorrectColorSet(currentColors);
                await UniTask.Yield();
            }            
            while (finalColors == null);
            previousColorSets.Add(finalColors);
            for (int i = 0; i < buckets.Count; i++)
            {
                buckets[i].color = finalColors[i];
                toysPivots[i].color = finalColors[i];
                buckets[i].GetComponent<DropZone>().Initialize(toysPivots[i].GetComponent<Draggable>());
            }
        }

        private List<Color> CorrectColorSet(List<Color> colors)
        {
            var firstThree = colors.GetRange(0, 3);
            if (previousColorSets.Count == 0)
            {
                return firstThree;
            }
            foreach (var item in previousColorSets)
            {
                if (firstThree.SequenceEqual(item)==true)
                {
                    return null;
                }
            }
            return firstThree;
        }

        private void SetToys()
        {
            for (int i = 0; i < toysPivots.Count; i++)
            {
                toysPivots[i].sprite = toys[^1];
                toys.Remove(toys[^1]);
            }
        }
    }
}
