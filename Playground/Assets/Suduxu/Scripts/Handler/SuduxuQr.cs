using System;
using UnityEngine;
using UnityEngine.UI;

public class SuduxuQr
{
    private RawImage qrImage;
    private Texture2D cachedTex;

    public SuduxuQr(RawImage qrImage)
    {
        this.qrImage = qrImage;
    }

    public void RefreshQrCode()
    {
        if (qrImage == null) return;

        QrResult result = SuduxuRaw.get_qr_code_rendered();

        if (result.ptr != IntPtr.Zero)
        {
            if (cachedTex == null || cachedTex.width != (int)result.width)
            {
                if (cachedTex != null) UnityEngine.Object.Destroy(cachedTex);
                cachedTex = new Texture2D((int)result.width, (int)result.width, TextureFormat.Alpha8, false);
                cachedTex.filterMode = FilterMode.Point;
                qrImage.texture = cachedTex;
            }

            cachedTex.LoadRawTextureData(result.ptr, (int)result.size);
            cachedTex.Apply();

            SuduxuRaw.free_qr_buffer(result.ptr, result.size);
        }
    }
}