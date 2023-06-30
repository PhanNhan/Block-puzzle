using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class XImage : MonoBehaviour
    {
        [SerializeField]
        private Image _image;

        private string _spritePath;
        private float _alpha = 1.0f;

        public string SpritePath
        {
            get
            {
                return _spritePath;
            }
            set
            {
                _spritePath = value;
                setSprite(_spritePath);
            }
        }

        public float Alpha
        {
            get { return _alpha; }
            set
            {
                _alpha = value;
                setAlpha(_alpha);
            }
        }

        public void SetNativeSize()
        {
            _image.SetNativeSize();
        }

        public Sprite Value
        {
            get { return _image.sprite; }
            set
            {
                _image.sprite = value;
            }
        }

        private void setSprite(string spritePath)
        {
            _image.sprite = ZPool.Assets.GetAsset<Sprite>(spritePath);
        }

        private void setAlpha(float alpha)
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, alpha);
        }

        public void SetColor(Color color)
        {
            if (_image == null)
                return;
            _image.color = color;
        }
    }
}
