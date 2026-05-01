using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SuduxuClient
{
    private static readonly string SDX_PREFIX = "[SDX]:";

    public event Action<ushort, Battery> OnBatteryChange;
    public event Action<ushort, Network> OnNetworkChange;

    public event Action<ushort> OnHealthy;
    public event Action<ushort> OnUnhealthy;
    public event Action<ushort> OnTimeout;

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
        Id = SuduxuId.BroadcastId;
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
                SuduxuRaw.broadcast(ptr).ToFFIStatus().ThrowIfException();
            else
                SuduxuRaw.unicast(Id, ptr).ToFFIStatus().ThrowIfException();
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
            $"{SDX_PREFIX}Vibrate",
            JToken.FromObject(new VibrationData(duration, strength, type))
        ));
    }

    public void PlaySound(string name)
    {
        _Send(new Payload(
            $"{SDX_PREFIX}PlaySound",
            JToken.FromObject(new PlaySound(name))
        ));
    }

    public void SendSensorData(bool enabled)
    {
        _Send(new Payload(
            $"{SDX_PREFIX}SendSensorData",
            JToken.FromObject(new SendSensorData(enabled))
        ));
    }

    public void SetFrameRate(ushort frameRate)
    {
        _Send(new Payload(
            $"{SDX_PREFIX}SetFrameRate",
            JToken.FromObject(new FrameRate(frameRate))
        ));
    }

    public void Log(LogLevel level, string message, string title = null)
    {
        _Send(new Payload(
            $"{SDX_PREFIX}Log",
            JToken.FromObject(new LogObject(level, message, title))
        ));
    }

    public void HandleState(EventObject evt)
    {
        ushort id = evt.value["id"]!.ToObject<ushort>();

        if (id != Id && Id != 0)
        {
            return;
        }

        switch (evt.kind)
        {
            case 0:
                OnBatteryChange?.Invoke(
                        evt.value["id"]!.ToObject<ushort>(),
                        evt.value["battery"]!.ToObject<Battery>()
                    );
                break;
            case 1:
                OnNetworkChange?.Invoke(
                        evt.value["id"]!.ToObject<ushort>(),
                        evt.value["network"]!.ToObject<Network>()
                    );
                break;
        }
    }

    public void HandleHealth(EventObject evt)
    {
        ushort id = evt.value["id"]!.ToObject<ushort>();

        if (id != Id && Id != 0)
        {
            return;
        }

        switch (evt.kind)
        {
            case 0:
                OnHealthy?.Invoke(id);
                break;
            case 1:
                OnUnhealthy?.Invoke(id);
                break;
            case 2:
                OnTimeout?.Invoke(id);
                break;
        }
    }
}