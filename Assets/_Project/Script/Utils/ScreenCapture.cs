using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
public class ScreenCapture : MonoBehaviour{
	public static IEnumerator TakeShot(Camera screenCamera, Action onComplete)
	{
		yield return new WaitForEndOfFrame();
		int resWidth ;
    	int resHeight ;
		resWidth = (int)screenCamera.pixelWidth;
		resHeight = (int)screenCamera.pixelHeight;
		RenderTexture renderTexture = new RenderTexture(resWidth, resHeight, 24);
		Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
		screenCamera.targetTexture = renderTexture;
		screenCamera.Render();

		RenderTexture.active = renderTexture; 
		screenShot.ReadPixels(new Rect(screenCamera.pixelRect), 0, 0);
		screenShot.Apply();
		byte[] bytes = screenShot.EncodeToPNG();
		string filename = Application.persistentDataPath + "/ThanksgivingScreenshot.png";
		System.IO.File.WriteAllBytes(filename, bytes);
		RenderTexture.active = null;
		Destroy(renderTexture);
		if(onComplete!=null)
			onComplete();
	}
}
