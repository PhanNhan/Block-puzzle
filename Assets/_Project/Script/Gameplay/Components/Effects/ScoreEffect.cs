using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Diagnostics;
using System;

namespace ClassicMode.Gameplay.Components
{
    public class ScoreEffect : MonoBehaviour, ZPool.IPoolListener
    {
        public Text Score;
        private Color originalColor = new Color(1, 1, 1, 0.5f);
        private Sequence scoreSquence;

        public void OnSpawn()
        {
            scoreSquence = null;
            originalColor = new Color(1, 1, 1, 0.5f);
            Score.text = "";
        }

        public void OnRecycle()
        {
            scoreSquence = null;
            originalColor = new Color(1, 1, 1, 0.5f);
            Score.text = "";
        }

        public void ShowScore(int score, int removeLineCount, Action onComplete = null)
        {
            originalColor = new Color(Score.color.r, Score.color.g, Score.color.b, 0.5f);
            switch (removeLineCount)
            {
                case 1:
                    Score.fontSize = 75;
                    break;
                case 2:
                    Score.fontSize = 80;
                    break;
                case 3:
                    Score.fontSize = 85;
                    break;
                case 4:
                    Score.fontSize = 90;
                    break;
                case 5:
                    Score.fontSize = 95;
                    break;
                case 6:
                    Score.fontSize = 100;
                    break;
                default:
                    Score.fontSize = 70;
                    break;
            }
            Score.text = "+ " + score.ToString();
            scoreSquence = DOTween.Sequence();
            scoreSquence.Append(transform.DOLocalMoveY(transform.localPosition.y + 150f, 0.3f).SetEase(Ease.InOutQuad));
            scoreSquence.Append(transform.DOLocalMoveY(transform.localPosition.y + 50f, 0.4f).SetEase(Ease.Linear));
            scoreSquence.Join(Score.DOColor(originalColor, 0.4f).SetEase(Ease.Linear));
            scoreSquence.OnComplete(() =>
            {
                if (onComplete != null)
                    onComplete();
                ZPool.Assets.Destroy(gameObject);
            });
        }
    }
}

