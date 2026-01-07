using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class SuduxuInput
{
    public ushort Id { get; private set; }

    // Sensor
    public delegate void SensorDataEvent(ref SensorDataRaw data);
    public event SensorDataEvent OnSensorData;

    // UDP
    public event Action OnUdpStart;
    public event Action OnUdpStop;
    public event Action<ushort, ButtonInput> OnButtonInput;
    public event Action<ushort, JoystickData> OnJoystickData;

    public SuduxuInput(ushort id)
    {
        Id = id;
    }

    public SuduxuInput For(ushort id)
    {
        Id = id;
        return this;
    }

    public SuduxuInput Broadcast()
    {
        Id = 0;
        return this;
    }

    public void OnSensorEvent(ref SensorDataRaw data)
    {
        if (data.id == Id)
        {
            OnSensorData?.Invoke(ref data);
        }
    }

    public void HandleUdp(EventObject evt)
    {
        switch (evt.kind)
        {
            case 0:
                OnUdpStart?.Invoke();
                break;

            case 1:
                OnButtonInput?.Invoke(
                    evt.value["id"]!.ToObject<ushort>(),
                    evt.value["input"]!.ToObject<ButtonInput>());
                break;

            case 2:
                OnUdpStop?.Invoke();
                break;

            case 3:
                OnJoystickData?.Invoke(
                    evt.value["id"]!.ToObject<ushort>(),
                    evt.value["input"]!.ToObject<JoystickData>());
                break;
        }
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
