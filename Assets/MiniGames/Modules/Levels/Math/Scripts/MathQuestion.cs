using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Random = UnityEngine.Random;

namespace MiniGames.Modules.Level.Math
{
    public class MathQuestion : MonoBehaviour
    {
        public event Action <int> questionReadyEvent;

        [Header("SeparateParts")]
        [SerializeField] private TMP_Text left;
        [SerializeField] private TMP_Text mathOperator;
        [SerializeField] private TMP_Text right;
        [SerializeField] private TMP_Text equalMark;
        [Space]
        [SerializeField] private int maxQuestionNumber = 5;
        private List<int> poolOfNumbers;
        private string[] lastQuestion;

        public TMP_Text Left => left;
        public TMP_Text MathOperator => mathOperator;
        public TMP_Text Right => right;
        public TMP_Text EqualMark => equalMark;

        private void Awake()
        {
            lastQuestion = new string[2];
        }

        public void Initialize(out List<int> poolOfNumbers )
        {
            this.poolOfNumbers = new List<int>();
            for (int i = 1; i <= maxQuestionNumber; i++)
            {
                this.poolOfNumbers.Add(i);
            }
            equalMark.text = "=";
            poolOfNumbers = this.poolOfNumbers;
        }

        public void SetupQuestion(Action onComplete)
        {
            int correctAnswer = 0;
            do
            {
                List<int> tmpNumbers = new(poolOfNumbers);
                int firstNumber = Random.Range(1, maxQuestionNumber);
                int secondNumber = Random.Range(1, maxQuestionNumber);
                if (Random.Range(0f, 1f) > 0.8f) //small chance to subtract question
                {
                    if (firstNumber == secondNumber)
                    {
                        tmpNumbers.Remove(secondNumber);
                        firstNumber = tmpNumbers[Random.Range(0, tmpNumbers.Count)];
                    }
                    int biggestNumber = firstNumber > secondNumber ? firstNumber : secondNumber;
                    int smallestNumber = biggestNumber == firstNumber ? secondNumber : firstNumber;
                    left.text = biggestNumber.ToString();
                    mathOperator.text = "-";
                    right.text = smallestNumber.ToString();
                    correctAnswer = biggestNumber - smallestNumber;
                }
                else //sum question
                {
                    left.text = firstNumber.ToString();
                    mathOperator.text = "+";
                    right.text = secondNumber.ToString();
                    correctAnswer = firstNumber + secondNumber;
                }
            } 
            while (string.Equals(left.text,lastQuestion[0])==true && string.Equals(right.text, lastQuestion[1]) == true);
            lastQuestion[0] = left.text;
            lastQuestion[1] = right.text;
            questionReadyEvent?.Invoke(correctAnswer);
            onComplete?.Invoke();
        }       
    }
}
