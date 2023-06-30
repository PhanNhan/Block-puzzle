using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerHelper : MonoBehaviour {
    void Awake()
    {
        var canvasScaler = this.gameObject.GetComponent<CanvasScaler> ();
        if (canvasScaler == null)
            return;

        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

        #if UNITY_IOS
        if (Util.Utils.IsIphoneX ()) {
            canvasScaler.matchWidthOrHeight = 0;
        } else {
            canvasScaler.matchWidthOrHeight = 1;
        }
        #else
        canvasScaler.matchWidthOrHeight = 0;
        #endif
    }
}
