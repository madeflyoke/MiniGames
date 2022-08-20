using MiniGames.Modules.Level.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace MiniGames.Modules.Level.XmasTree
{
    public class XmasTreeController : MonoBehaviour
    {
        [Serializable]
        public struct Star
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

        [Header("Toys And Cells")]
        [Tooltip("Star must go first and set up by player last, so done separation")]
        [SerializeField] private Star star;
        [SerializeField] private List<Transform> toysPivots;
        [SerializeField] private List<ToyCellPair> toyCellPairs;
        [Header("Toys Bag")]
        [SerializeField] private Transform bagPivot;
        [SerializeField] private Transform endRevealPivot;
        private CancellationTokenSource cancellationToken;
        private Vector3 bagDefaultScale;

        private void Awake()
        {
            bagDefaultScale = bagPivot.localScale;
            cancellationToken = new CancellationTokenSource();
            SetupToysCells();
            SetupToysBag();
        }

        private void SetupToysCells()
        {
            foreach (var item in toyCellPairs) //drag n drop initialize
            {
                item.toyCell.Initialize(item.toy);
            }

            List<int> rndNumbers = new(); //pool for random numbers 
            
            for (int i = 0; i < toyCellPairs.Count; i++)
            {
                rndNumbers.Add(i);
            }

            for (int i= 0; i < toysPivots.Count; i++)
            {
                int rnd = rndNumbers[Random.Range(0,rndNumbers.Count)];         
                toyCellPairs[rnd].toyCell.transform.SetParent(toysPivots[i]);
                toyCellPairs[rnd].toyCell.transform.position = toysPivots[i].transform.position;
                rndNumbers.Remove(rnd);
            }

            star.starCell.transform.SetParent(star.starPivot);
            star.starCell.transform.position = star.starPivot.position;
            star.starCell.Initialize(star.starToy);
            star.starCell.correctAnswerEvent += OnLastAnswerDone;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                ShowToy();
            }
        }

        private void SetupToysBag()
        {
            foreach (var item in toyCellPairs)
            {
                item.toy.gameObject.SetActive(false);
                item.toy.transform.position = bagPivot.transform.position;
            }
        }

        private void ShowToy()
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(bagPivot.DOScaleX(bagDefaultScale.x * 1.1f, 0.3f))
                .Join(bagPivot.DOScaleY(bagDefaultScale.y * 0.9f, 0.3f))
                .Append(bagPivot.DOScaleX(bagDefaultScale.x * 0.88f, 0.2f))
                .Join(bagPivot.DOScaleY(bagDefaultScale.y * 1.13f, 0.2f))
                .Append(bagPivot.DOScaleX(bagDefaultScale.x, 0.8f))
                .Join(bagPivot.DOScaleY(bagDefaultScale.y, 0.8f)).SetEase(Ease.InCubic);
                
           
        }

        private void OnLastAnswerDone()
        {

        }
     
    }

}
