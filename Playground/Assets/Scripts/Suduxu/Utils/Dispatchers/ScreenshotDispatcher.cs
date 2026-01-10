using System.Collections;

public class ScreenshotDispatcher
{
    public static IEnumerator TakeScreenshotAndNotify(string path, ushort id)
    {
        UnityEngine.ScreenCapture.CaptureScreenshot(path);

        while (!System.IO.File.Exists(path))
            yield return null;

        SuduxuRaw.notify_screenshot(path, id);
    }
}