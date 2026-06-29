using System;

/// <summary>
/// Central class used for managing server events.
/// </summary>
public class SuduxuServer
{
    /// <summary>
    /// Event for when the TCP server starts.
    /// </summary>
    public event Action OnTcpStart;

    /// <summary>
    /// Event for when the TCP server stops.
    /// </summary>
    public event Action OnTcpStop;

    /// <summary>
    /// Event for when a client completes the TCP handshake and connects to the server (after authentication if enabled).
    /// Passed parameter is the ID of the connecting client.
    /// </summary>
    public event Action<ushort> OnClientConnected;

    /// <summary>
    /// Event for when a client disconnects from the server.
    /// Passed parameter is the ID of the disconnecting client.
    /// </summary>
    public event Action<ushort> OnClientDisconnected;

    /// <summary>
    /// Event for when a client receives a payload type that is not supported by the server.
    /// Passed parameter is the payload which was sent to the client.
    /// </summary>
    public event Action<Payload> OnIllegalSuduxuMethod;

    public void HandleTcp(EventObject evt)
    {
        switch (evt.kind)
        {
            case 0:
                OnTcpStart?.Invoke();
                break;

            case 1:
                OnClientConnected?.Invoke(evt.value["id"]!.ToObject<ushort>());
                break;

            case 2:
                OnClientDisconnected?.Invoke(evt.value["id"]!.ToObject<ushort>());
                break;

            case 4:
                OnIllegalSuduxuMethod?.Invoke(
                    evt.value["payload"]!.ToObject<Payload>());
                break;

            case 5:
                OnTcpStop?.Invoke();
                break;
        }
    }
}