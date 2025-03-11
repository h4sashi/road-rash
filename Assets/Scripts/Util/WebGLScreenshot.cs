using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class WebGLScreenshot : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void SaveScreenshot(byte[] data, int length);

    public void CaptureScreenshot()
    {
        StartCoroutine(Capture());
    }

    public IEnumerator Capture()
    {
        yield return new WaitForEndOfFrame();

        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] pngData = screenshot.EncodeToPNG();
        //string base64Image = System.Convert.ToBase64String(pngData);

#if UNITY_WEBGL && !UNITY_EDITOR
            SaveScreenshot(pngData, pngData.Length);
#endif

        Object.Destroy(screenshot);
    }
}
