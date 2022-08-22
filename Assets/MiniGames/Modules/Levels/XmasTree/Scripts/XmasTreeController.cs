using MiniGames.Modules.Level.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;

namespace MiniGames.Modules.Level.XmasTree
{
    public class XmasTreeController : MonoBehaviour
    {
        [Serializable]
        public struct StarData
        {
            public DropZone starCell;
            public Draggable starToy;
            public Transform starPivot;
        }

        [Serializable]
        public struct ToyCellPair
        {
            public DropZone toyCell;
            public Draggable toy;
        }

        [Header("Particles")]
        [SerializeField] private ParticleSystem winEffect;
        [SerializeField] private ParticleSystem snow;
        [Space]
        [SerializeField] private Scratcher scratcher;
        [SerializeField] private XmasTreeAnimator animator;
        [SerializeField] private ToysBagController toysBagController;
        [SerializeField] private BackToMenuSlider backToMenuSlider;
        [Header("Toys And Cells")]
        [Tooltip("Star must go first and set up by player last, so done separation")]
        [SerializeField] private StarData star;
        [SerializeField] private List<Transform> toysPivots;
        [SerializeField] private List<ToyCellPair> toyCellPairs;
        public BackToMenuSlider BackToMenuSlider => backToMenuSlider;
        public List<Transform> ToysPivots => toysPivots;
        public StarData Star => star;
        public List<ToyCellPair> ToyCellPairs => toyCellPairs;
        public GraphicRaycaster Raycaster { get; private set; }
        private CancellationTokenSource cancellationToken;

        private void Awake()
        {
            Raycaster = GetComponent<GraphicRaycaster>();
            Raycaster.enabled = false;
            cancellationToken = new CancellationTokenSource();
            snow.Play();
            winEffect.gameObject.SetActive(false);
            SetupToysCells();          
        }

        private void Start()
        {
            animator.Initialize();
            toysBagController.Initialize(); 
        }

        public void StartGame()
        {          
            animator.ShowingAnimation(async () =>
            {
                await UniTask.Delay(200, cancellationToken: cancellationToken.Token);
                toysBagController.ShowHelper();
                Raycaster.enabled = true;
                backToMenuSlider.gameObject.SetActive(true);
            });
        }

        private void SetupToysCells()
        {
            foreach (var item in toyCellPairs) //drag n drop initialize
            {
                item.toyCell.Initialize(item.toy);
            }

            List<int> rndNumbers = new(); //tmp pool for random numbers 

            for (int i = 0; i < toyCellPairs.Count; i++)
            {
                rndNumbers.Add(i);
            }

            for (int i = 0; i < toysPivots.Count; i++)
            {
                int rnd = rndNumbers[Random.Range(0, rndNumbers.Count)];
                toyCellPairs[rnd].toyCell.transform.SetParent(toysPivots[i]);
                toyCellPairs[rnd].toyCell.transform.position = toysPivots[i].transform.position;
                rndNumbers.Remove(rnd);
            }

            star.starCell.transform.SetParent(star.starPivot);
            star.starCell.transform.position = star.starPivot.position;
            star.starCell.Initialize(star.starToy);
            star.starCell.correctAnswerEvent += OnLastAnswerDone;
        }

        private async void OnLastAnswerDone()
        {
            backToMenuSlider.gameObject.SetActive(false);
            await UniTask.Delay(250, cancellationToken: cancellationToken.Token);
            winEffect.gameObject.SetActive(true);
            winEffect.Play();
            await UniTask.WaitUntil(() => winEffect.gameObject.activeInHierarchy == false, cancellationToken: cancellationToken.Token);
            scratcher.StartScratching();
            await UniTask.Delay(3000, cancellationToken: cancellationToken.Token);
            gameObject.SetActive(false);
        }
    }
}
