using System.Collections;
using UnityEngine;

public class ScreenshotDispatcher
{
    public static IEnumerator TakeScreenshotAndNotify(string path, ushort id)
    {
        UnityEngine.ScreenCapture.CaptureScreenshot(path);

        while (!System.IO.File.Exists(path))
            yield return null;

        Debug.Log(path);

        SuduxuRaw.notify_screenshot(path, id).ToFFIStatus().ThrowIfException();
    }
}