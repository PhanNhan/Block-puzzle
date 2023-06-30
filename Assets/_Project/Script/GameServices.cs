using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Profile;

public static class GameServices
{
    private static ProfileService profileService;
    private static GameDataService gameDataService;

    public static ProfileService ProfileService
    {
        get
        {
            return profileService;
        }
    }

    public static GameDataService GameDataService
    {
        get
        {
            return gameDataService;
        }
    }

    public static bool HasInitialize = false;

    public static IEnumerator Init()
    {
        profileService = new ProfileService();
        gameDataService = Profile.GameDataService.CreateIns();

        yield return profileService.Init();
        yield return new UnityEngine.WaitForSeconds(0.5f);
        HasInitialize = true;
    }
}
