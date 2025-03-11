using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class ShareGameScreenshot : MonoBehaviour
{
    [SerializeField] private string gameURL = "https://yourgameurl.com";
    private const int IMAGE_SCALE_FACTOR = 2; // Reduces the resolution for iOS compatibility

    [DllImport("__Internal")]
    private static extern void ShareImageWithURL(string imageData, string url);

    public void ShareGame()
    {
        StartCoroutine(CaptureScreenshotAndShare());
    }

    private System.Collections.IEnumerator CaptureScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();

        // Reduce screenshot resolution (helps with iOS compatibility)
        // int width = Screen.width / IMAGE_SCALE_FACTOR;
        // int height = Screen.height / IMAGE_SCALE_FACTOR;
        // Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        // screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        // screenshot.Apply();

        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] imageBytes = screenshot.EncodeToPNG();
        string base64Image = Convert.ToBase64String(imageBytes);

        Destroy(screenshot);

#if UNITY_WEBGL && !UNITY_EDITOR
            ShareImageWithURL(base64Image, gameURL);
#else
        Debug.Log("Sharing is only available in WebGL.");
#endif
    }
}
