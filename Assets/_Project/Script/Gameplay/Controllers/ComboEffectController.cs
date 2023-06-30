using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using ClassicMode.Gameplay.Components;
using ClassicMode.Gameplay.Controllers;
using MyEventSystem;
using Util;

namespace ClassicMode.Gameplay.Controllers
{
    public class ComboEffectController : MonoBehaviour
    {
        [SerializeField]
        private Transform _effectsParent;
        [SerializeField]
        private GameObject _scoreEffect;

        public void ShowComboEffect(int totalPoint, int combo, Vector2 position)
        {
            //bool isShowCombo = false;
            if (Math.Abs(position.x) > Screen.width / 2 - Screen.width / 7)
            {
                float newPosX = Screen.width / 2 - Screen.width / 7;
                position = new Vector2(position.x < 0 ? -newPosX : newPosX, position.y);
            }
        }

        public void InstanceScoreEffect(int score, int removeLineCount, Vector3 position, Action onComplete)
        {
            GameObject effect = ZPool.Assets.Clone(_scoreEffect);
            effect.transform.SetParent(_effectsParent);
            effect.transform.localScale = Vector3.one;
            effect.transform.localPosition = position;
            effect.GetComponent<ScoreEffect>().ShowScore(score, removeLineCount, onComplete);
        }
    }
}
