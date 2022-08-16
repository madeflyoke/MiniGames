using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace MiniGames.Modules.Level.Math
{
    public class MathAnswerVariant : MonoBehaviour
    {
        [SerializeField] private Image colorizedImage;
        [SerializeField] private TMP_Text answerText;
        public Image ColorizedImage => colorizedImage;
        public TMP_Text AnswerText => answerText;
    }


}

