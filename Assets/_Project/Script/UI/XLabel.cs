using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class XLabel : MonoBehaviour
    {
        [SerializeField]
        private Text _text;

        [SerializeField]
        private string _value;

        [SerializeField]
        private bool _isUpcase = false;

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
            }
        }

        void Awake()
        {

        }

        private void setValue(string value)
        {
            if (_text == null)
                throw new System.NullReferenceException();

            if (value == "")
                throw new System.InvalidOperationException();

            _text.text = _isUpcase ? value.ToUpper() : value;
        }
    }
}
