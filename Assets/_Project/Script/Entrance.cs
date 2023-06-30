using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Entrance : MonoBehaviour
{

    [SerializeField]
    private GameObject loadingScreen;
    // Start is called before the first frame update
    private void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        Input.multiTouchEnabled = false;
        DOTween.Init();
        StartCoroutine(InitLocalData());
    }

    IEnumerator InitLocalData()
    {
        yield return GameServices.Init();
        AsyncOperation asyncGameplay = SceneManager.LoadSceneAsync("Gameplay", LoadSceneMode.Additive);
        yield return asyncGameplay.isDone;
        GameObject.Destroy(gameObject);
    }
}
