using System;
using UnityEngine;

public class SuduxuServer
{
    public event Action OnTcpStart;
    public event Action OnTcpStop;

    public event Action<ushort> OnClientConnected;
    public event Action<ushort> OnClientDisconnected;

    public event Action<string> OnError;
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

            case 3:
                OnError?.Invoke(evt.value["message"]!.ToObject<string>());
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