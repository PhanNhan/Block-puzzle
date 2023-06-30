using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ClassicMode.Gameplay.Controllers;

namespace ClassicMode.Gameplay.Components
{
    public class TileComponent : MonoBehaviour
    {
        public Image Background;
        public Image Highlight;

        public void Init()
        {
        }

        public void Refresh()
        {
        }

        public void SetEnableHighlight(bool enable)
        {
            if (enable)
            {
                Highlight.DOFade(0.2f, 0.0f);
            }
            Highlight.gameObject.SetActive(enable);
        }
    }
}
