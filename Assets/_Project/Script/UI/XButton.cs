using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace UI
{
    public class XButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
    {
        [SerializeField]
        private Image _optImage = null;
        [SerializeField]
        private string _optAudioId = null;
        [SerializeField]
        private UnityEngine.UI.Button _optButton = null;
        [SerializeField]
        private UnityEngine.UI.Text _optText = null;

        private List<Action<XButton>> _delegates = new List<Action<XButton>>();

        private Action<XButton> _realOnClicked;
        private bool _isEnable = true;
        private string _text = "";

        public event Action<XButton> OnClicked
        {
            add
            {
                _realOnClicked += value;
                _delegates.Add(value);
            }

            remove
            {
                _realOnClicked -= value;
                _delegates.Remove(value);
            }
        }

        public Color Color
        {
            set
            {
                if (_optImage != null)
                {
                    _optImage.color = value;
                }
            }
        }

        public Sprite SpriteBackground
        {
            set
            {
                _optImage.sprite = value;
            }
            get
            {
                return _optImage.sprite;
            }
        }

        public bool Enable
        {
            get { return _isEnable; }
            set
            {
                _isEnable = value;
                if (_optButton != null)
                {
                    _optButton.interactable = _isEnable;
                }
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                if (_optText != null)
                {
                    _optText.text = _text;
                }
            }
        }

        public Image ButtonImage
        {
            get
            {
                return _optImage;
            }
        }

        public void RemoveAllListeners()
        {
            for (int i = 0, c = _delegates.Count; i < c; ++i)
            {
                var d = _delegates[i];
                _realOnClicked -= d;
            }
            _delegates.Clear();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isEnable) return;
            var audioId = string.IsNullOrEmpty(_optAudioId) ? C.AudioIds.Sound.ButtonClick : _optAudioId;
            AudioPlayer.Instance.PlaySound(audioId);
            if (_realOnClicked != null) _realOnClicked(this);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isEnable) return;
        }

        public void EnableInteracable(bool _isEnable)
        {
            if (_optButton == null)
                return;
            _optButton.interactable = _isEnable;
        }

        public void EnableColorTint()
        {
            if (_optButton == null)
                return;
            _optButton.transition = Button.Transition.ColorTint;
        }
    }
}
