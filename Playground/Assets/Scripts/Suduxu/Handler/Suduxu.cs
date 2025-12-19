using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Newtonsoft.Json;

public class Suduxu
{
    public SuduxuInput Input { get; }
    public SuduxuClient Client { get; }
    public SuduxuServer Server { get; }
    public SuduxuLog Log { get; }

    private static SuduxuRaw.EventCallback _eventCallback;
    private static SuduxuRaw.SensorEventCallback _sensorCallback;

    public Suduxu(ushort defaultClientId)
    {
        Input = new SuduxuInput(defaultClientId);
        Client = new SuduxuClient(defaultClientId);
        Server = new SuduxuServer();
        Log = new SuduxuLog();
    }

    public void RegisterCallbacks()
    {
        _eventCallback = OnEvent;
        _sensorCallback = Input.OnSensorEvent;

        SuduxuRaw.register_event_callback(_eventCallback);
        SuduxuRaw.register_sensor_event_callback(_sensorCallback);
    }

    public void Launch()
    {
        if (IsRunning())
            throw new SuduxuException("Suduxu is already running.");

        SuduxuRaw.serverThread = new Thread(() =>
        {
            SuduxuRaw.start_suduxu();
        })
        {
            IsBackground = true
        };

        SuduxuRaw.serverThread.Start();
    }

    public void Stop()
    {
        if (!IsRunning())
            throw new SuduxuException("Suduxu is not running.");

        SuduxuRaw.stop_suduxu();

        if (SuduxuRaw.serverThread != null && SuduxuRaw.serverThread.IsAlive)
        {
            SuduxuRaw.serverThread.Join();
            SuduxuRaw.serverThread = null;
        }
    }

    public bool IsRunning()
    {
        return SuduxuRaw.is_running();
    }

    public void UseClient(ushort id)
    {
        Input.For(id);
        Client.For(id);
    }
    public List<SuduxuDevice> FindAllClients()
    {
        return ReadJson<List<SuduxuDevice>>(SuduxuRaw.find_all_clients);
    }

    public SuduxuDevice FindClientById(ushort id)
    {
        SuduxuDevice device =
            ReadJson<SuduxuDevice>(() => SuduxuRaw.find_client_by_id(id));

        return device;
    }

    public void DisconnectAll()
    {
        SuduxuRaw.disconnect_all();
    }

    public void DisconnectClient(ushort id)
    {
        SuduxuRaw.disconnect_client(id);
    }

    private void OnEvent(IntPtr ptr)
    {
        string json = Marshal.PtrToStringAnsi(ptr);
        SuduxuRaw.free(ptr);

        EventObject evt = JsonConvert.DeserializeObject<EventObject>(json);

        if (evt.type == "Log")
        {
            Log.Handle(evt);
        }
        else if (evt.type == "Udp")
        {
            Input.HandleUdp(evt);
        }
        else if (evt.type == "Tcp")
        {
            Server.HandleTcp(evt);
        }
    }

    private T ReadJson<T>(Func<IntPtr> nativeCall)
    {
        IntPtr ptr = nativeCall();
        try
        {
            string json = Marshal.PtrToStringAnsi(ptr);
            return JsonConvert.DeserializeObject<T>(json);
        }
        finally
        {
            SuduxuRaw.free(ptr);
        }
    }
}
