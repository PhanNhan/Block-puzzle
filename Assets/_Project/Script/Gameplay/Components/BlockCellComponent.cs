using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ClassicMode.Gameplay.Controllers;
using Util;

namespace ClassicMode.Gameplay.Components
{
    public class BlockCellComponent : MonoBehaviour, ZPool.IPoolListener
    {
        public Image Background;
        public Image ElementalTexture;
        public CanvasGroup Group;

        private int _type;
        private bool isHighlight = false;
        private float baseAlpha = 1.0f;
        private bool _canGray = false;

        private Tweener _animHighlight;

        public int Type { get { return _type; } }

        public void OnSpawn()
        {
            isHighlight = false;
            _canGray = false;
            Group.alpha = 1.0f;
            transform.localScale = Vector3.one;
            transform.eulerAngles = Vector3.zero;
            SetPivot(Vector2.one * 0.5f);
            Background.gameObject.SetActive(true);
            ElementalTexture.gameObject.SetActive(true);

            if (_animHighlight != null)
                _animHighlight.Kill();
        }

        public void OnRecycle() { }

        public void Init(int type)
        {
            _type = type;
            updateView();
        }

        public void SetHighlight(int type)
        {
            _type = type;
            isHighlight = true;
            updateView();
        }

        public void EnableAnimationHightlight(bool enabled)
        {
            if (_animHighlight != null)
                _animHighlight.Kill();

            if (enabled)
            {
                Group.alpha = 0.01f;
                _animHighlight = Group.DOFade(0.8f, 0.3f).SetLoops(-1, LoopType.Yoyo);
            }
        }

        public void Show()
        {
            {
                var color = Background.color;
                color.a = baseAlpha * 0.3f;
                Background.color = color;
                Background.DOFade(baseAlpha, 0.5f);
            }

            {
                var color = ElementalTexture.color;
                color.a = 0.3f;
                ElementalTexture.color = color;
                ElementalTexture.DOFade(1.0f, 0.5f);
            }
        }

        public void Explosion(float delayInSeconds, System.Action onComplete)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOScale(Vector3.one * 0.5f, 0.15f).SetEase(Ease.InFlash));
            seq.Join(Group.DOFade(0.0f, 0.3f));
            seq.SetDelay(delayInSeconds);
            seq.OnComplete(() =>
            {
                AudioPlayer.Instance.PlaySound(C.AudioIds.Sound.DestroyBlock);
                Background.gameObject.SetActive(false);
                ElementalTexture.gameObject.SetActive(false);
                onComplete();
            });
        }

        public void SetPivot(Vector2 pivot)
        {
            Background.GetComponent<RectTransform>().pivot = (pivot);
            ElementalTexture.GetComponent<RectTransform>().pivot = (pivot);
        }

        public void Refresh()
        {
            updateView();
        }

        private void updateView()
        {
            baseAlpha = Background.color.a;

            Background.DOFade(isHighlight ? baseAlpha * 0.55f : baseAlpha, 0.0f);
            ElementalTexture.DOFade(isHighlight ? 0.55f : 1.0f, 0.0f);
        }

        public void SetGray()
        {
            Background.color = Color.gray;
            if (_canGray)
                ElementalTexture.color = Color.gray;
        }

        public void SetGray(float delay = 0.25f)
        {
            Background.DOColor(Color.gray, 0.25f).SetDelay(delay).SetEase(Ease.Linear);
            if (_canGray)
                ElementalTexture.DOColor(Color.gray, 0.25f).SetDelay(delay).SetEase(Ease.Linear);

        }
    }
}
