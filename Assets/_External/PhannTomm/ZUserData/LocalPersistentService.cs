using System.IO;
using System.Text;
using UnityEngine;

namespace ZUserData
{
    public class LocalPersistentService<T> : ILocalPersistentService<T>
        where T : class, new()
    {
        private T _obj = null;
        private string _fullPath = null;
        private bool _useEncrypt = false;
        private System.Action<T> OnInitDefaultValue;

        public LocalPersistentService(string fullPath, bool useEncrypt, System.Action<T> onInitDefaultValue = null)
        {
            _fullPath = fullPath;
            _useEncrypt = useEncrypt;
            OnInitDefaultValue = onInitDefaultValue;

#if UNITY_IOS
//            if (!isSetNoBackupFlagInICloud())
            {
                setNoBackupFlagInICloud();
            }
#endif
        }

        public LocalPersistentService(string fullPath) : this(fullPath, useEncrypt: true, onInitDefaultValue: null) { }

        public LocalPersistentService(string fullPath, string content, bool useEncypt)
            : this(fullPath, useEncrypt: true, onInitDefaultValue: null)
        {
            SetRaw(content);
        }

        public T Get()
        {
            if (_obj == null)
            {
                throw new System.InvalidOperationException();
            }
            return _obj;
        }

        public void Save()
        {
            string json = JsonService.ToJson<T>(_obj);
            if (_useEncrypt)
            {
                json = EncryptionUtil.Encrypt(json);
            }

            StreamWriter sw = File.CreateText(_fullPath);
            sw.Write(json);
            sw.Close();
        }

        public void Load()
        {
            if (_obj != null) return;

            if (File.Exists(_fullPath))
            {
                var str = File.ReadAllText(_fullPath);
                if (_useEncrypt)
                {
                    str = EncryptionUtil.Decrypt(str);
                }

                _obj = JsonService.ToObject<T>(str);
            }
            else
            {
                Renew();
            }
        }

        public void Renew()
        {
            _obj = new T();
            if (OnInitDefaultValue != null)
            {
                OnInitDefaultValue(_obj);
            }
            Save();
        }

        public string ToRaw()
        {
            return JsonService.ToJson<T>(_obj);
        }

        public void SetRaw(string content)
        {
            _obj = JsonService.ToObject<T>(content);
            Save();
        }

        private bool isSetNoBackupFlagInICloud()
        {
            return PlayerPrefs.GetInt(_fullPath, 0) >= 1;
        }

        private void setNoBackupFlagInICloud()
        {
#if UNITY_IOS
            UnityEngine.iOS.Device.SetNoBackupFlag(_fullPath);
#endif
            PlayerPrefs.SetInt(_fullPath, 9999);
            PlayerPrefs.Save();
        }
    }
}
