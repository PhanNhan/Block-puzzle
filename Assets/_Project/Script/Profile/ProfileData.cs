using System.Collections.Generic;
using UnityEngine;
using System;

namespace Profile
{
    public class ProfileData
    {
        public string VersionPrevious;

        //Classic mode
        public int BestScore;
        public int TotalScore;
        public int CountTimePlay;
        public bool IsRemoveAds;
        public bool IsRemoveAdsGameCenter;
        public bool IsRestoreIAP;
        public System.DateTime TimeEndCountDownWatchAds;
        public System.DateTime TimeEndCountDownGiftAds;
        public System.DateTime TimeAskNotification;
        public string CurrentTheme;
        public List<string> Themes;
        public EThemeColor ThemeColor;
        public int TutorialPlayTimes;

        public DateTime TimeUsingTheme;

        public int SoundValue;
        public int MusicValue;
        public string LanguageCode;
        public int Coins;
        public int CurrentLevel;

        public bool IsShowedRateGame;
        public bool IsRatedGame;
        public int RateStar;

        public bool IsFirstClaimDaily;
        public bool EnableNotification;
        public System.DateTime PrevLoginTime;

        public ProfileData()
        {
            TimeUsingTheme = DateTime.Now;
        }

        public void Init()
        {
            VersionPrevious = Application.version;
            Themes = new List<string>();
            ThemeColor = EThemeColor.Night;
            TutorialPlayTimes = 0;
            IsRemoveAds = false;
            IsRemoveAdsGameCenter = false;
            IsRestoreIAP = false;
            BestScore = 0;
            TotalScore = 0;
            Coins = 0;
            MusicValue = SoundValue = 3;
            RateStar = 0;
            IsFirstClaimDaily = true;
            LanguageCode = "en";
            EnableNotification = false;
            IsShowedRateGame = IsRatedGame = false;
            CountTimePlay = 0;
            TimeEndCountDownGiftAds = System.DateTime.Now;
            TimeEndCountDownWatchAds = System.DateTime.Now;
        }

        public enum EThemeColor
        {
            Light,
            Night
        }
    }
}