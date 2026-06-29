using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Central class used for interacting with clients and retrieving client data.
/// </summary>
public class SuduxuClient
{
    private static readonly string SDX_PREFIX = "[SDX]:";

    /// <summary>
    /// Event for when a client's battery status changes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This event is emitted only for the specific client ID associated with the event.
    /// If the system is configured in broadcast mode, all matching subscribers receive updates,
    /// but the event payload still contains the originating client ID.
    /// </para>
    /// <para>
    /// Parameters:
    /// </para>
    /// <para>
    /// - <c>id</c>: The ID of the client whose battery status changed.
    /// </para>
    /// <para>
    /// - <c>battery</c>: The updated battery state of the client.
    /// </para>
    /// </remarks>
    public event Action<ushort, Battery> OnBatteryChange;

    /// <summary>
    /// Event for when a client's network status changes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This event is emitted only for the specific client ID associated with the event.
    /// In broadcast mode, updates from all clients are forwarded to subscribers, but each event
    /// still includes the originating client ID.
    /// </para>
    /// <para>
    /// Parameters:
    /// </para>
    /// <para>
    /// - <c>id</c>: The ID of the client whose network status changed.
    /// </para>
    /// <para>
    /// - <c>network</c>: The updated network state of the client.
    /// </para>
    /// </remarks>
    public event Action<ushort, Network> OnNetworkChange;

    /// <summary>
    /// Event for when a client is marked as healthy.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Emitted when a client's health state transitions to healthy.
    /// Only events matching subscribed client IDs are emitted unless broadcast mode is enabled.
    /// </para>
    /// <para>
    /// Parameter:
    /// </para>
    /// <para>
    /// - <c>id</c>: The ID of the client marked as healthy.
    /// </para>
    /// </remarks>
    public event Action<ushort> OnHealthy;

    /// <summary>
    /// Event for when a client is marked as unhealthy.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Emitted when a client's health state transitions to unhealthy.
    /// Only events matching subscribed client IDs are emitted unless broadcast mode is enabled.
    /// </para>
    /// <para>
    /// Parameter:
    /// </para>
    /// <para>
    /// - <c>id</c>: The ID of the client marked as unhealthy.
    /// </para>
    /// </remarks>
    public event Action<ushort> OnUnhealthy;

    /// <summary>
    /// Event for when a client times out.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Emitted when a client fails to respond within the expected timeout window.
    /// Only events matching subscribed client IDs are emitted unless broadcast mode is enabled.
    /// </para>
    /// <para>
    /// Parameter:
    /// </para>
    /// <para>
    /// - <c>id</c>: The ID of the client that timed out.
    /// </para>
    /// </remarks>
    public event Action<ushort> OnTimeout;

    public ushort Id { get; private set; }

    public SuduxuClient(ushort id)
    {
        Id = id;
    }

    /// <summary>
    /// Changes the client that subsequent commands will be sent to.
    /// </summary>
    /// <param name="id">
    /// ID of the target client.
    /// </param>
    /// <returns>
    /// The current <see cref="SuduxuClient"/> instance.
    /// </returns>
    public SuduxuClient For(ushort id)
    {
        Id = id;
        return this;
    }

    /// <summary>
    /// Configures this instance to broadcast commands to all connected clients.
    /// </summary>
    /// <remarks>
    /// All supported commands invoked through this instance will be sent to every
    /// connected client.
    /// </remarks>
    /// <returns>
    /// The current <see cref="SuduxuClient"/> instance.
    /// </returns>
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

    /// <summary>
    /// Triggers a vibration on the selected client or clients.
    /// </summary>
    /// <param name="duration">
    /// Duration of the vibration in milliseconds.
    /// </param>
    /// <param name="strength">
    /// Strength of the vibration.
    /// </param>
    /// <param name="type">
    /// Type of vibration motor to activate.
    /// </param>
    public void Vibrate(long duration, VibrationStrength strength, VibrationType type)
    {
        _Send(new Payload(
            $"{SDX_PREFIX}Vibrate",
            JToken.FromObject(new VibrationData(duration, strength, type))
        ));
    }

    /// <summary>
    /// Requests the client to play a bundled sound.
    /// </summary>
    /// <param name="name">
    /// Name of the sound to play.
    /// </param>
    public void PlaySound(string name)
    {
        _Send(new Payload(
            $"{SDX_PREFIX}PlaySound",
            JToken.FromObject(new PlaySound(name))
        ));
    }

    /// <summary>
    /// Enables or disables streaming of sensor data from the client.
    /// </summary>
    /// <param name="enabled">
    /// <see langword="true"/> to enable sensor data streaming;
    /// otherwise, <see langword="false"/>.
    /// </param>
    public void SendSensorData(bool enabled)
    {
        _Send(new Payload(
            $"{SDX_PREFIX}SendSensorData",
            JToken.FromObject(new SendSensorData(enabled))
        ));
    }

    /// <summary>
    /// Sets the target frame rate of the client application.
    /// </summary>
    /// <param name="frameRate">
    /// Desired frame rate in frames per second.
    /// </param>
    public void SetFrameRate(ushort frameRate)
    {
        _Send(new Payload(
            $"{SDX_PREFIX}SetFrameRate",
            JToken.FromObject(new FrameRate(frameRate))
        ));
    }

    /// <summary>
    /// Displays a log message on the client.
    /// </summary>
    /// <param name="level">
    /// Severity of the log message.
    /// </param>
    /// <param name="message">
    /// Message to display.
    /// </param>
    /// <param name="title">
    /// Optional title shown together with the message.
    /// </param>
    public void Log(LogLevel level, string message, string title = null)
    {
        _Send(new Payload(
            $"{SDX_PREFIX}Log",
            JToken.FromObject(new LogObject(level, message, title))
        ));
    }

    /// <summary>
    /// Sends a file transfer command to a specific client or broadcasts it to all clients when the client id is 0.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This command instructs the receiving side to resolve and load a file that is defined in the
    /// <c>file_sharing</c> section of the <c>suduxu.json</c> configuration.
    /// The file is not sent as raw content here; instead, a logical file identifier/path is transmitted,
    /// and the remote side is responsible for resolving it.
    /// </para>
    /// <para>
    /// If the underlying client instance was created via broadcast mode (<c>id == 0</c>), the command is
    /// dispatched to all connected clients.
    /// </para>
    /// </remarks>
    /// <param name="path">
    /// Logical file identifier or path entry defined in the <c>file_sharing</c> configuration section.
    /// This is not necessarily a filesystem path on the remote machine.
    /// </param>
    public void File(string path)
    {
        _Send(new Payload(
            $"{SDX_PREFIX}File",
            JToken.FromObject(new FileTransferObject(path))
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