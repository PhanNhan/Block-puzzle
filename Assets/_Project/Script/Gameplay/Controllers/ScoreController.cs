using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MyEventSystem;
using ClassicMode.Gameplay.Components;
using Profile;
using DG.Tweening;
using UI;
using System;

namespace ClassicMode.Gameplay.Controllers
{
    public class ScoreController : MonoBehaviour, IRefreshable
    {
        public event Action OnNewBestScore;

        public TileController TileController;
        public ComboEffectController ComboEffectController;
        public XText ScoreText;
        public XText BestScoreText;
        private bool _hasShowBestScore;

        public int Score { get; private set; }

        public int OldScore { get; private set; }

        public long[] ComboCount;
        public const int MAX_COMBO = 6;

        public void Init()
        {
            TileController.OnTileChanged += onTileChanged;
        }

        public void CleanUp()
        {
            TileController.OnTileChanged -= onTileChanged;
        }

        public void OnReset()
        {


            Score = 0;
            OldScore = 0;

            BestScoreText.Value = "*******";
            _hasShowBestScore = false;
            ScoreText.Value = Score.ToString();

            OnRefresh();

            ComboCount = new long[MAX_COMBO + 1];
            for (int i = 0; i < MAX_COMBO + 1; i++)
                ComboCount[i] = 0;
        }

        public void ResetComboCount()
        {
            ComboCount = new long[MAX_COMBO + 1];
            for (int i = 0; i < MAX_COMBO + 1; i++)
                ComboCount[i] = 0;
        }

        public void OnRefresh()
        {
            Debug.LogError("REFRESH");
        }

        public void OnLoadGameData(GameData gameData)
        {
            _hasShowBestScore = gameData.IsBreakBestScore;
            Score = gameData.Score;
            ScoreText.Value = Score.ToString();
        }

        public void OnSaving(GameData gameData)
        {
            gameData.IsBreakBestScore = _hasShowBestScore;
            gameData.Score = Score;
        }

        private void onTileChanged(BlockComponent block, int row, int col, int removedLineCount)
        {
            if (removedLineCount > 0 && removedLineCount < MAX_COMBO + 1)
            {
                ComboCount[removedLineCount]++;
            }
            int totalPoint = block.CellCount() + (int)(removedLineCount * removedLineCount * C.GameplayConfig.ScorePerLine);
            ComboEffectController.ShowComboEffect((int)(totalPoint), removedLineCount, TileController.PointAtIndex(row, col));
            updateScoreText(totalPoint);
        }

        private void updateScoreText(int deltaScore)
        {
            OldScore = Score;
            Score += deltaScore;
            ScoreText.transform.DOScale(Vector3.one * 1.5f, C.GameplayConfig.ScoreChangingTime).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                DOTween.To(() => OldScore, x => updateScoreTextAnim(x), Score, C.GameplayConfig.ScoreChangingTime).OnComplete(() =>
                {
                    ScoreText.transform.DOScale(Vector3.one, C.GameplayConfig.ScoreChangingTime).SetEase(Ease.Linear);
                    if (Score > 0)
                    {
                        BestScoreText.Value = Score.ToString();
                        if (!_hasShowBestScore)
                        {
                            _hasShowBestScore = true;
                            if (OnNewBestScore != null)
                                OnNewBestScore();
                        }
                    }

                });
            });

        }

        private void updateScoreTextAnim(float value)
        {
            OldScore = (int)value;
            int tempScore = (int)value;
            ScoreText.Value = tempScore.ToString();
        }
    }
}
