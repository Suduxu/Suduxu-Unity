using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class SuduxuInput
{
    public ushort Id { get; private set; }
    private readonly SuduxuConfig _config;

    // Sensor
    public delegate void SensorDataEvent(ushort id, SensorDataRaw data);
    public event SensorDataEvent OnSensorData;

    // UDP
    public event Action OnUdpStart;
    public event Action OnUdpStop;
    public event Action<ushort, ButtonInput> OnButtonInput;
    public event Action<ushort, JoystickData> OnJoystickData;
    public event Action<ushort, string> OnScreenshot;

    public SuduxuInput(ushort id, SuduxuConfig config)
    {
        Id = id;
        _config = config;
    }

    public SuduxuInput For(ushort id)
    {
        Id = id;
        return this;
    }

    public SuduxuInput Broadcast()
    {
        Id = SuduxuId.BroadcastId;
        return this;
    }

    public void OnSensorEvent(IntPtr sensorDataPtr)
    {
        var data = Marshal.PtrToStructure<SensorDataRaw>(sensorDataPtr);

        if (data.id == Id || Id == SuduxuId.BroadcastId)
        {
            MainThreadDispatcher.Enqueue(() => OnSensorData?.Invoke(data.id, data));
        }
    }

    public void HandleUdp(EventObject evt)
    {
        ushort id;

        switch (evt.kind)
        {
            case 0:
                OnUdpStart?.Invoke();
                break;

            case 1:
                id = evt.value["id"]!.ToObject<ushort>();

                if (id != Id && Id != 0)
                {
                    return;
                }

                OnButtonInput?.Invoke(
                    id,
                    evt.value["input"]!.ToObject<ButtonInput>());
                break;

            case 2:
                OnUdpStop?.Invoke();
                break;

            case 3:
                id = evt.value["id"]!.ToObject<ushort>();

                if (id != Id && Id != 0)
                {
                    return;
                }

                OnJoystickData?.Invoke(
                    id,
                    evt.value["input"]!.ToObject<JoystickData>());
                break;
            case 4:
                id = evt.value["id"]!.ToObject<ushort>();
                string path = evt.value["path"]!.ToObject<string>();

                if (id != Id && Id != 0)
                {
                    return;
                }

                OnScreenshot?.Invoke(
                    id,
                    path);

                if (_config.screenCapture.enabled)
                {
                    CoroutineDispatcher.Instance.Run(
                        ScreenshotDispatcher.TakeScreenshotAndNotify(path, id)
                    );
                }

                break;
        }
    }

    public bool GetButtonReleased(ButtonInputType type)
    {
        return SuduxuRaw.get_button_in_state(Id, type, ButtonInputState.Up) ||
               SuduxuRaw.get_button_in_state(Id, type, ButtonInputState.Cancel);
    }

    public bool GetButtonDown(ButtonInputType type)
    {
        return SuduxuRaw.get_button_in_state(Id, type, ButtonInputState.Down);
    }

    public bool GetButton(ButtonInputType type)
    {
        return
            SuduxuRaw.get_button_in_state(Id, type, ButtonInputState.Pressed) ||
            SuduxuRaw.get_button_in_state(Id, type, ButtonInputState.Down);
    }

    public bool GetButtonUp(ButtonInputType type)
    {
        return SuduxuRaw.get_button_in_state(Id, type, ButtonInputState.Up);
    }

    public bool GetButtonInState(ButtonInputType type, ButtonInputState state)
    {
        return SuduxuRaw.get_button_in_state(Id, type, state);
    }

    public void Tick(float delta)
    {
        SuduxuRaw.tick(delta);
    }
}