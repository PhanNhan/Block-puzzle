using UnityEngine;
using TMPro;

namespace UI
{
    public class XText : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _text = null;
        private string _value;

        public void Awake()
        {
            if (_text == null) throw new System.NullReferenceException();
        }

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                callSetter();
            }
        }

        public Color Color
        {
            set { _text.color = value; }
        }

        private void callSetter()
        {
            _text.text = _value;
        }

        public float FontSize
        {
            get { return _text.fontSize; }
            set
            {
                _text.fontSize = value;
            }
        }
    }
}
