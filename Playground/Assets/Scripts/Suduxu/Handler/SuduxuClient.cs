using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class SuduxuClient
{
    public ushort Id { get; private set; }

    public SuduxuClient(ushort id)
    {
        Id = id;
    }

    public SuduxuClient For(ushort id)
    {
        Id = id;
        return this;
    }

    public SuduxuClient Broadcast()
    {
        Id = 0;
        return this;
    }

    private void _Send(Payload payload)
    {
        IntPtr ptr = IntPtr.Zero;

        try
        {
            string json = JsonConvert.SerializeObject(payload);
            ptr = Marshal.StringToHGlobalAnsi(json);

            if (Id == 0)
                SuduxuRaw.broadcast(ptr);
            else
                SuduxuRaw.unicast(Id, ptr);
        }
        finally
        {
            if (ptr != IntPtr.Zero)
                Marshal.FreeHGlobal(ptr);
        }
    }

    public void Vibrate(long duration, VibrationStrength strength, VibrationType type)
    {
        _Send(new Payload(
            "Vibrate",
            JToken.FromObject(new VibrationData(duration, strength, type))
        ));
    }

    public void PlaySound(string name)
    {
        _Send(new Payload(
            "PlaySound",
            JToken.FromObject(new PlaySound(name))
        ));
    }

    public void SendSensorData(bool enabled)
    {
        _Send(new Payload(
            "SendSensorData",
            JToken.FromObject(new SendSensorData(enabled))
        ));
    }

    public void SetFrameRate(ushort frameRate)
    {
        _Send(new Payload(
            "SetFrameRate",
            JToken.FromObject(new FrameRate(frameRate))
        ));
    }

    public void Log(LogLevel level, string message, string title = null)
    {
        var payload = new Payload(
            "Log",
            JToken.FromObject(new LogObject(level, message, title))
        );

        Debug.Log(payload);

        _Send(payload);
    }
}