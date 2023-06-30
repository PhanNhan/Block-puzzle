using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class C
{
    public static class GameplayConfig
    {
        public const int Rows = 9;
        public const int Columns = 9;

        public const int ScorePerLine = 10;
        public const float ScoreChangingTime = 0.25f;

        public const float BlockCellWidth = 92f;
        public const float BlockCellHeight = 92f;

        public const float RemoveCellDelayInSeconds = 0.05f;

        public const float ComboEffectInSeconds = 1.0f;
        public const bool GameModeOneBlock = false;
    }
    
    public static class PlayerPrefKeys
    {
        public const string IsEnableMusic = "is_enable_music";
        public const string IsEnableSound = "is_enable_sound";
        public const string CountryCode = "country_code";
        public const string CountryName = "country_name";
        public const string FirstTimeOpend = "first_time_opened";
    }

    public static class AudioIds
    {
        public static class Sound
        {
            public const string ButtonClick = "sound_button_clicked";
            public const string DropBlock = "sound_drop_block";
            public const string GameOver = "sound_game_over";
            public const string GameOver2 = "sound_game_over2";
            public const string WrongBlock = "sound_wrong_block_1";
            public const string WrongBlock1 = "sound_wrong_block_2";
            public const string DestroyBlock = "sound_destroy_block";
            public const string SwitchColor = "sound_switch_color";
            public const string CollectDiamond = "sound_collect_diamond";
            public const string BestScore = "sound_best_score";
        }
    }

    public static class FilePaths
    {
        public static readonly string Profile = Application.persistentDataPath + "/3421869983870292";
        public static readonly string DataGame = Application.persistentDataPath + "/2749942636713470";
        public static readonly string Tracking = Application.persistentDataPath + "/3423423523556743";
    }
    
    public static readonly float[] BlockHeights = { 1, 1, 1, 1, 1, 2, 3, 4, 5, 2, 2, 2, 2, 3, 3, 3, 3, 2, 3, 3, 2, 3, 2, 3, 2, 3, 2, 2, 3, 2, 3, 2, 3, 2, 3 };
}
