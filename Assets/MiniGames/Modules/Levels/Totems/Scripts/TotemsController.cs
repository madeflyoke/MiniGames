using MiniGames.Extensions;
using MiniGames.Modules.Level.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace MiniGames.Modules.Level.Totems
{
    public class TotemsController : LevelController
    {
        private const int _levelsCount = 5;

        [SerializeField] private TotemsAnimator animator;
        [SerializeField] private TotemsHelper totemsHelper;
        [SerializeField] private Button playerWigwam;
        [SerializeField] private ParticleSystem winEffect;
        [SerializeField] private ScratchAgainButton scratchAgainButton;
        [SerializeField] private List<ParticleSystem> answerZonesParticles;
        [SerializeField] private List<Image> answers;
        [SerializeField] private List<DropZone> answerFields;
        [SerializeField] private List<Sprite> totems;
        private int maxAnswers;
        private int count=0;
        private CancellationTokenSource cts;
        private Dictionary<DropZone, ParticleSystem> answersParticlesDict;

        private void Awake()
        {
            cts = new();
            answersParticlesDict = new();
            maxAnswers = _levelsCount * answers.Count;
            totems = totems.Shuffle();
            playerWigwam.onClick.AddListener(WigwamListener);
            playerWigwam.interactable = false;
            winEffect.gameObject.SetActive(false);
        }

        private void Start()
        {
            for (int i = 0; i < answerFields.Count; i++)
            {
                DropZone target = answerFields[i];
                answerFields[i].correctAnswerEvent +=()=> CheckAnswers(target);
                answersParticlesDict[answerFields[i]] = answerZonesParticles[i];
            }
            totemsHelper.InitializeFirstPointer(() => playerWigwam.interactable == false);
            TutorialLogic();
        }

        private async void TutorialLogic()
        {
            await UniTask.WaitUntil(() => playerWigwam.interactable == true, cancellationToken:cts.Token);
            totemsHelper.InitializeSecondPointer(answers[count].transform,
    answerFields.Where(x => x.CorrectObject.gameObject == answers[count].gameObject).FirstOrDefault().transform,
    totemsHelper.DefaultStopTrigger);
            totemsHelper.ShowHelper();
        }

        public override void StartGame()
        {
            backToMenuSlider.gameObject.SetActive(true);
            SetTotemTask();
        }

        private void SetTotemTask()
        {
            List<Sprite> correctTotems = totems.GetRange(0, 3);
            totems.RemoveRange(0, 3);
            for (int i = 0; i < answerFields.Count; i++)
            {
                answers[i].sprite = correctTotems[i];
                answerFields[i].Initialize(answers[i].GetComponent<Draggable>());
            }
            answers = answers.Shuffle();
            animator.ShowAnimation(correctTotems, () =>
            {
                playerWigwam.interactable = true;
            });
        }

        private async void CheckAnswers(DropZone correctAnswerZone)
        {
            answersParticlesDict[correctAnswerZone].Play();
            maxAnswers--;
            if (maxAnswers<=0)
            {
                animator.SetCheckMark();
                OnLastAnswerDone();
            }
            else if (maxAnswers % answers.Count == 0)
            {
                answerZonesParticles.Clear();
                count = 0;
                await UniTask.Delay(500, cancellationToken: cts.Token);
                animator.SetCheckMark();
                animator.CorrectTaskAnswerAnimation(async() => 
                {
                    await UniTask.Delay(500, cancellationToken: cts.Token);
                    animator.HideAnimation(async() =>
                    {
                        await UniTask.Delay(1000, cancellationToken: cts.Token);
                        SetTotemTask();
                    });                                            
                });
            }
            else
                playerWigwam.interactable = true;
        }

        private async void OnLastAnswerDone()
        {
            if (isNeedReward)
                backToMenuSlider.gameObject.SetActive(false);
            await UniTask.Delay(250, cancellationToken: cts.Token);
            winEffect.gameObject.SetActive(true);
            winEffect.Play();
            await UniTask.WaitUntil(() => winEffect.gameObject.activeInHierarchy == false, cancellationToken: cts.Token);
            if (isNeedReward)  //check whether its first walkthrough or not
                scratcher.StartScratching();
            else
            {
                scratchAgainButton.Activate();
                await UniTask.WaitUntil(() => scratchAgainButton.Button.interactable == false, cancellationToken: cts.Token);
            }
            await UniTask.Delay(3000, cancellationToken: cts.Token);
            gameObject.SetActive(false);
        }

        private void WigwamListener()
        {
            playerWigwam.interactable = false;
            animator.PlayerWigwamAnimation(() =>
            {
                answers[count].gameObject.SetActive(true);
                count++;
            });
        }

        private void OnDestroy()
        {
            if (cts!=null)
            {
                cts.Cancel();
            }
        }
    }
}

