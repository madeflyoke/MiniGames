using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MiniGames.Modules.Level.Utils;
using Random = UnityEngine.Random;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace MiniGames.Modules.Level.Math
{
    public class MathController : LevelController
    {
        [SerializeField] private ScratchAgainButton scratchAgainButton;
        [SerializeField] private float nextQuestionDelay;
        [Tooltip("Need synchronization with check marks on screen ")]
        [SerializeField] private int questionsCount;

        [Header("VFX")]
        [SerializeField] private ParticleSystem correctAnswerEffect;
        [SerializeField] private ParticleSystem winEffect;
        [Space]
        [SerializeField] private List<MathAnswerVariant> answers;
        [SerializeField] private List<Color> cardColors;
        [SerializeField] private MathQuestion question;
        [SerializeField] private DropZone answerZone;
        [SerializeField] private MathAnimator animator;
        private GraphicRaycaster canvasRaycaster;
        private List<int> poolOfNumbers;
        private CancellationTokenSource cancellationToken;

        private void Awake()
        {
            cancellationToken = new CancellationTokenSource();
            canvasRaycaster = GetComponent<GraphicRaycaster>();
            canvasRaycaster.enabled = false;
            question.Initialize(out poolOfNumbers);
            correctAnswerEffect.transform.position = answerZone.transform.position + (Vector3.back*5);
            winEffect.gameObject.SetActive(false);
            winEffect.transform.position = Vector3.zero + (Vector3.forward * 5);
        }

        public override void StartGame()
        {
            question.SetupQuestion(() => animator.ShowingAnimation(() => {
                canvasRaycaster.enabled = true;
                backToMenuSlider.gameObject.SetActive(true);
            }));
            questionsCount--;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            question.questionReadyEvent += SetupVariants;
            answerZone.correctAnswerEvent += CorrectAnswerLogic;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            question.questionReadyEvent -= SetupVariants;
            answerZone.correctAnswerEvent -= CorrectAnswerLogic;
        }

        private async void CorrectAnswerLogic()
        {
            animator.SetCheckMark();
            correctAnswerEffect.Play();
            canvasRaycaster.enabled = false;
            questionsCount--;         
            if (questionsCount < 0) //end
            {
                EndLogic();
                return;
            }
            await UniTask.Delay(1500, cancellationToken: cancellationToken.Token); //time between questions
            animator.HidingAnimation(async() =>
            {
                answerZone.gameObject.SetActive(true);
                await UniTask.Delay((int)(nextQuestionDelay * 1000), cancellationToken: cancellationToken.Token);
                question.SetupQuestion(
                () => animator.ShowingAnimation(
                    () => canvasRaycaster.enabled = true));
             });
        }

        public void SetupVariants(int correctAnswer)
        {
            List<Color> tmpColors = new(cardColors);
            List<int> tmpNumbers = new(poolOfNumbers);
            int correctIndex = Random.Range(0, answers.Count);
            tmpNumbers.Remove(correctAnswer);

            for (int i = 0; i < answers.Count; i++)
            {
                Color color = tmpColors[Random.Range(0, tmpColors.Count)];
                answers[i].ColorizedImage.color = color;
                tmpColors.Remove(color);

                if (correctIndex == i)
                {
                    answers[i].AnswerText.text = correctAnswer.ToString();
                    answerZone.Initialize(answers[i].GetComponent<Draggable>());
                }
                else
                {
                    int wrongAnswer = tmpNumbers[Random.Range(0, tmpNumbers.Count)];
                    answers[i].AnswerText.text = wrongAnswer.ToString();
                    tmpNumbers.Remove(wrongAnswer);
                }
            }
        }

        private async void EndLogic()
        {
            if (isNeedReward)
                backToMenuSlider.gameObject.SetActive(false);
            winEffect.gameObject.SetActive(true);
            winEffect.Play();
            await UniTask.WaitUntil(() => winEffect.gameObject.activeInHierarchy == false, cancellationToken: cancellationToken.Token);
            if (isNeedReward)
                scratcher.StartScratching();
            else
            {
                canvasRaycaster.enabled = true;
                scratchAgainButton.Activate();
                await UniTask.WaitUntil(() => scratchAgainButton.Button.interactable == false, cancellationToken: cancellationToken.Token);
            }
            await UniTask.Delay(3000, cancellationToken: cancellationToken.Token);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            //Draggable.s_currentDraggable = null;
        }
    }
}

