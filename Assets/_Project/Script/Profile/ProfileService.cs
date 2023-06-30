using System.Collections;
using ZUserData;
using UnityEngine;
using System;
using System.Collections.Generic;
using MyEventSystem;

namespace Profile
{
    public class ProfileService
    {
        private ILocalPersistentService<ProfileData> _localPersistentService;
        private DelaySaver _delaySaver;
        private ProfileData _profile;

        public ProfileService()
        {
            var delaySaverGameObj = new UnityEngine.GameObject("ZDelaySaver_ProfileData");
            _delaySaver = delaySaverGameObj.AddComponent<DelaySaver>();
            _delaySaver.Save = forceSave;

#if UNITY_EDITOR
            _localPersistentService = new LocalPersistentService<ProfileData>(
                C.FilePaths.Profile, useEncrypt: false, onInitDefaultValue: initDefaultValue);
#else
            _localPersistentService = new LocalPersistentService<ProfileData>(
                C.FilePaths.Profile, useEncrypt: true, onInitDefaultValue: initDefaultValue);
#endif
            _localPersistentService.Load();
        }
        public int BestScore { get { return _profile.BestScore; } }
        public int TotalScore { get { return _profile.TotalScore; } }
        public bool IsRemoveAds { get { return _profile.IsRemoveAds; } }
        public bool IsRemoveAdsGameCenter { get { return _profile.IsRemoveAdsGameCenter; } }
        public bool IsRestoreIAP { get { return _profile.IsRestoreIAP; } }
        public System.DateTime TimeEndCountDownWatchAds { get { return _profile.TimeEndCountDownWatchAds; } }
        public System.DateTime TimeEndCountDownGiftAds { get { return _profile.TimeEndCountDownGiftAds; } }

        public bool HasPlayTutorial { get { return _profile.TutorialPlayTimes > 0; } }
        public int TotalCoins { get { return _profile.Coins; } }
        public int SoundValue { get { return _profile.SoundValue; } }
        public int MusicValue { get { return _profile.MusicValue; } }
        public int CountTimePlay { get { return _profile.CountTimePlay; } set { _profile.CountTimePlay = value; save(); } }
        public string VersionPrevious { get { return _profile.VersionPrevious; } set { _profile.VersionPrevious = value; save(); } }

        public int CurrentLevel
        {
            set { _profile.CurrentLevel = value; }
            get { return _profile.CurrentLevel; }
        }

        public bool IsShowRating { get { return _profile.IsShowedRateGame; } }
        public bool IsRatedGame { get { return _profile.IsRatedGame; } }
        public int RateStar { get { return _profile.RateStar; } }

        public bool IsFirstClaimDaily { get { return _profile.IsFirstClaimDaily; } }
        public bool EnableNotification { get { return _profile.EnableNotification; } }
        public System.DateTime PrevLoginTime { get { return _profile.PrevLoginTime; } }

        // For Halloween
        public IEnumerator Init()
        {
            _profile = _localPersistentService.Get();
            yield return new WaitForSeconds(0.1f);
            yield return new WaitForEndOfFrame();
        }

        private void initDefaultValue(ProfileData profile)
        {
            profile.Init();
        }

        public void SetTotalCoin(int value)
        {
            if (value < 0)
                return;
            _profile.Coins = value;
            save();
            EventObsever.Instance.BroadcastEvent(GameEvent.GE_COIN_CHANGED);
        }

        public void SetEnbleNotification(bool isEnable)
        {
            _profile.EnableNotification = isEnable;
            save();
        }

        public void SetFirstClaimDaily(bool isEnable)
        {
            _profile.IsFirstClaimDaily = isEnable;
            save();
        }

        public void SetPrevLoginTime(System.DateTime dateTime)
        {
            _profile.PrevLoginTime = dateTime;
            save();
        }

        public void AddCoin(int coins)
        {
            _profile.Coins += coins;
            save();

            EventObsever.Instance.BroadcastEvent(GameEvent.GE_THEME_UPDATE_UNLOCK);
            EventObsever.Instance.BroadcastEvent(GameEvent.GE_COIN_CHANGED);
        }

        public void ConsumeCoin(int coins)
        {
            if (_profile.Coins < coins) throw new System.InvalidOperationException();
            _profile.Coins -= coins;
            save();

            EventObsever.Instance.BroadcastEvent(GameEvent.GE_THEME_UPDATE_UNLOCK);
            EventObsever.Instance.BroadcastEvent(GameEvent.GE_COIN_CHANGED);
        }

        public void PassTutorial()
        {
            _profile.TutorialPlayTimes++;
            save();
        }

        public void SetBestScore(int score)
        {
            if (_profile.BestScore > score) throw new System.InvalidOperationException();
            _profile.BestScore = score;
            save();

            EventObsever.Instance.BroadcastEvent(GameEvent.GE_THEME_UPDATE_UNLOCK);
        }

        public void SetTotalScore(int total)
        {
            _profile.TotalScore = total;
            save();
        }

        public void SetMusicValue(int value)
        {
            if (value < 0)
                throw new IndexOutOfRangeException();
            _profile.MusicValue = value;
            save();
        }

        public void SetSoundValue(int value)
        {
            if (value < 0)
                throw new IndexOutOfRangeException();
            _profile.SoundValue = value;
            save();
        }

        public void SetShowedRateGame(bool showed)
        {
            _profile.IsShowedRateGame = showed;
            save();
        }

        public void SetRateStar(int star)
        {
            _profile.RateStar = star;
            save();
        }

        public void SetRateGame(bool rated)
        {
            _profile.IsRatedGame = rated;
            save();
        }

        public void SetRemoveAds(bool isRemoveAds)
        {
            _profile.IsRemoveAds = isRemoveAds;
            save();
        }

        public void SetRemoveAdsGameCenter(bool isRemoveAdsGameCenter)
        {
            _profile.IsRemoveAdsGameCenter = isRemoveAdsGameCenter;
            save();
        }

        public void SetRestoreIAP(bool isRestoreIAP)
        {
            _profile.IsRestoreIAP = isRestoreIAP;
            save();
        }

        public void SetTimeEndCountDownWatchAds(System.DateTime timeEndCountDownWatchAds)
        {
            _profile.TimeEndCountDownWatchAds = timeEndCountDownWatchAds;
            save();
        }

        public void SetTimeEndCountDownGiftAds(System.DateTime timeEndCountDownGiftAds)
        {
            _profile.TimeEndCountDownGiftAds = timeEndCountDownGiftAds;
            save();
        }

        public void BuyMoreTurn(int price)
        {
            if (_profile.Coins < price) throw new System.InvalidOperationException();
            _profile.Coins -= (int)price;

            EventObsever.Instance.BroadcastEvent(GameEvent.GE_COIN_CHANGED);
        }

        private void save()
        {
            _delaySaver.DelaySave();
        }

        private void forceSave()
        {
            _delaySaver.CancelCurrentDelaySaveIfAny();
            _localPersistentService.Save();
        }
    }
}
