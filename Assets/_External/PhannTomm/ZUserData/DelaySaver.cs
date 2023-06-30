using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZUserData
{
    public class DelaySaver : MonoBehaviour
    {
        private const float DelayInSeconds = 30.0f;

        private Coroutine _currentCor = null;

        public Action Save;

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void DelaySave()
        {
            if (!this) return;

            if (_currentCor == null) _currentCor = StartCoroutine(waitThenSave());
        }

        public void CancelCurrentDelaySaveIfAny()
        {
            if (!this) return;

            if (_currentCor != null)
            {
                StopCoroutine(_currentCor);
                _currentCor = null;
            }
        }

        public void OnApplicationPause(bool isPaused)
        {
            try
            {
                if (isPaused && _currentCor != null)
                {
                    CancelCurrentDelaySaveIfAny();
                    save();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Tracking Save Profile Fail");
            }
        }

        public void OnApplicationQuit()
        {
            if (_currentCor != null)
            {
                CancelCurrentDelaySaveIfAny();
                save();
            }
        }

        private IEnumerator waitThenSave()
        {
            yield return new WaitForSeconds(DelayInSeconds);
            save();
            _currentCor = null;
        }

        private void save()
        {
            if (Save != null) Save();
        }
    }
}
