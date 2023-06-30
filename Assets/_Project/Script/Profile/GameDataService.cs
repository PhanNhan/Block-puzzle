using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZUserData;
using System;

namespace Profile
{
    public class GameDataService : MonoBehaviour
    {
        public static GameDataService CreateIns()
        {
            var gameObj = new GameObject("GameDataService");
            var instance = gameObj.AddComponent<GameDataService>();
            instance.init();
            return instance;
        }

        public Action<GameData> OnSaving;

        private ILocalPersistentService<GameData> _localPersistentService;
        private GameData _data;

        public void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void init()
        {
#if UNITY_EDITOR
            _localPersistentService = new LocalPersistentService<GameData>(C.FilePaths.DataGame, useEncrypt: false);
#else
            _localPersistentService = new LocalPersistentService<GameData>(C.FilePaths.DataGame, useEncrypt: true);
#endif

            _localPersistentService.Load();
            _data = _localPersistentService.Get();
        }

        public bool HasSaved { get { return _data.HasSaved; } }

        public void ClearGameData()
        {
            _data.HasSaved = false;
            save();
        }

        public GameData GetGameData()
        {
            return _data;
        }

        public void OnApplicationPause(bool isPaused)
        {
            try
            {
                if (isPaused)
                {
                    SaveGame();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Tracking bug save game");
            }
        }

        public void SaveGame()
        {
            if (OnSaving != null)
            {
                OnSaving(_data);
            }
            save();
        }

        private void save()
        {
            _localPersistentService.Save();
        }
    }
}
