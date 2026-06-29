using System;
using System.Runtime.InteropServices;

/// <summary>
/// Central class used for retrieving input from specified clients
/// </summary>
public class SuduxuInput
{
    /// <summary>
    /// Id of which's client's input is retrieved.
    /// </summary>
    public ushort Id { get; private set; }
    private SuduxuConfig Config
    {
        get
        {
            return SuduxuConfig.Instance;
        }
    }

    // Sensor
    public delegate void SensorDataEvent(ushort id, SensorDataRaw data);

    /// <summary>
    /// Event for when sensor data is received from a client
    /// Passed parameter is the raw sensor data received from the client (including the client ID and whether the data is mocked (`mocked == 0` means real data, `mocked == 1` means mocked data)).
    /// </summary>
    public event SensorDataEvent OnSensorData;

    // UDP

    /// <summary>
    /// Event for when the UDP server starts.
    /// </summary>
    public event Action OnUdpStart;

    /// <summary>
    /// Event for when the UDP server stops.
    /// </summary>
    public event Action OnUdpStop;

    /// <summary>
    /// <para>
    /// Event for when a button input is received from a client (if not set to broadcast, only events with specified ID's get emitted).
    /// </para>
    /// <para>
    /// Passed parameters are:
    /// </para>
    /// <para>
    /// - <c>id</c>: The ID of the client that sent the button input.
    /// </para>
    /// <para>
    /// - <c>input</c>: The button input data received from the client.
    /// </para>
    /// </summary>
    public event Action<ushort, ButtonInput> OnButtonInput;

    /// <summary>
    /// <para>
    /// Event for when joystick data is received from a client (if not set to broadcast, only events with specified ID's get emitted).
    /// </para>
    /// <para>
    /// Passed parameters are:
    /// </para>
    /// <para>
    /// - <c>id</c>: The ID of the client that sent joystick data.
    /// </para>
    /// <para>
    /// - <c>input</c>: The joystick data received from the client.
    /// </para>
    /// </summary>
    public event Action<ushort, JoystickData> OnJoystickData;

    /// <summary>
    /// <para>
    /// Event for when a screenshot is triggered by a client (if not set to broadcast, only events with specified ID's get emitted).
    /// </para>
    /// <para>
    /// Passed parameters are:
    /// </para>
    /// <para>
    /// - <c>id</c>: The ID of the client that triggered the screenshot.
    /// </para>
    /// <para>
    /// - <c>path</c>: Path to the screenshot that got saved.
    /// </para>
    /// </summary>
    public event Action<ushort, string> OnScreenshot;

    public SuduxuInput(ushort id)
    {
        Id = id;
    }

    /// <summary>
    /// Changes the client whose input will be retrieved by this instance.
    /// </summary>
    /// <param name="id">
    /// ID of the client whose input should be received.
    /// </param>
    /// <returns>
    /// The current <see cref="SuduxuInput"/> instance.
    /// </returns
    public SuduxuInput For(ushort id)
    {
        Id = id;
        return this;
    }

    /// <summary>
    /// Configures this instance to receive input from all connected clients.
    /// </summary>
    /// <remarks>
    /// Events emitted by this instance will no longer be filtered by client ID.
    /// </remarks>
    /// <returns>
    /// The current <see cref="SuduxuInput"/> instance.
    /// </returns>
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

                if (Config.screenCapture.enabled)
                {
                    CoroutineDispatcher.Instance.Run(
                        ScreenshotDispatcher.TakeScreenshotAndNotify(path, id)
                    );
                }

                break;
        }
    }

    /// <summary>
    /// Returns whether the specified button was released during the current frame.
    /// </summary>
    /// <param name="type">
    /// The button to query.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the button entered the
    /// <see cref="ButtonInputState.Up"/> or
    /// <see cref="ButtonInputState.Cancel"/> state;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool GetButtonReleased(ButtonInputType type)
    {
        return SuduxuRaw.get_button_in_state(Id, type, ButtonInputState.Up) ||
               SuduxuRaw.get_button_in_state(Id, type, ButtonInputState.Cancel);
    }

    /// <summary>
    /// Returns whether the specified button was pressed during the current frame.
    /// </summary>
    /// <param name="type">
    /// The button to query.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the button entered the
    /// <see cref="ButtonInputState.Down"/> state;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool GetButtonDown(ButtonInputType type)
    {
        return SuduxuRaw.get_button_in_state(Id, type, ButtonInputState.Down);
    }

    /// <summary>
    /// Returns whether the specified button is currently pressed.
    /// </summary>
    /// <param name="type">
    /// The button to query.
    /// </param>
    /// <returns>
    /// <see langword="true"/> while the button is held,
    /// including the frame it was initially pressed.
    /// </returns>
    public bool GetButton(ButtonInputType type)
    {
        return
            SuduxuRaw.get_button_in_state(Id, type, ButtonInputState.Pressed) ||
            SuduxuRaw.get_button_in_state(Id, type, ButtonInputState.Down);
    }

    /// <summary>
    /// Returns whether the specified button entered the
    /// <see cref="ButtonInputState.Up"/> state during the current frame.
    /// </summary>
    /// <param name="type">
    /// The button to query.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the button was released during the current frame;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool GetButtonUp(ButtonInputType type)
    {
        return SuduxuRaw.get_button_in_state(Id, type, ButtonInputState.Up);
    }

    /// <summary>
    /// Returns whether the specified button is currently in the given state.
    /// </summary>
    /// <param name="type">
    /// The button to query.
    /// </param>
    /// <param name="state">
    /// The state to check.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the button is currently in the specified state;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool GetButtonInState(ButtonInputType type, ButtonInputState state)
    {
        return SuduxuRaw.get_button_in_state(Id, type, state);
    }

    /// <summary>
    /// Advances the internal button state machine.
    /// </summary>
    /// <remarks>
    /// This method should be called once per frame, typically from
    /// <c>MonoBehaviour.Update()</c>, using <c>Time.deltaTime</c>.
    /// </remarks>
    /// <param name="delta">
    /// Time elapsed since the previous frame, in seconds.
    /// </param>
    public void Tick(float delta)
    {
        SuduxuRaw.tick(delta);
    }
}