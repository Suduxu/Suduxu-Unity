using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

[Injectable]
public class Suduxu : MonoBehaviour
{
    [SerializeField]
    private ushort defaultId = 0;

    public ushort DefaultId => defaultId;
    public SuduxuInput Input { get; private set; }
    public SuduxuClient Client { get; private set; }
    public SuduxuServer Server { get; private set; }
    public SuduxuLog Log { get; private set; }

    private static SuduxuRaw.EventCallback _eventCallback;
    private static SuduxuRaw.SensorEventCallback _sensorCallback;

    JsonSerializerSettings settings = new JsonSerializerSettings
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        },
        Converters =
        {
            new Newtonsoft.Json.Converters.StringEnumConverter()
        },
        NullValueHandling = NullValueHandling.Include,
        MissingMemberHandling = MissingMemberHandling.Error,
        DefaultValueHandling = DefaultValueHandling.Populate
    };

    private SuduxuConfig _config;

    public SuduxuConfig Config {
        get
        {
            if (_config == null)
            {
                _config = GetConfig();
            }

            return _config;
        }
    }

    public AddressObject Addresses => GetAddresses();

    public uint? Password => Config.security.password;

    private void Awake()
    {
        Input = new SuduxuInput(defaultId);
        Client = new SuduxuClient(defaultId);
        Server = new SuduxuServer();
        Log = new SuduxuLog();
    }

    private SuduxuConfig GetConfig()
    {
        return _ReadJson<SuduxuConfig>(SuduxuRaw.config, settings);
    }

    private AddressObject GetAddresses()
    {
        return _ReadJson<AddressObject>(SuduxuRaw.addresses);
    }

    private void RegisterCallbacks()
    {
        _eventCallback = _OnEvent;
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

        RegisterCallbacks();
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
        return _ReadJson<List<SuduxuDevice>>(SuduxuRaw.find_all_clients);
    }

    public SuduxuDevice FindClientById(ushort id)
    {
        SuduxuDevice device =
            _ReadJson<SuduxuDevice>(() => SuduxuRaw.find_client_by_id(id));

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

    private void _OnEvent(IntPtr ptr)
    {
        string json = Marshal.PtrToStringAnsi(ptr);
        SuduxuRaw.free(ptr);

        EventObject evt = JsonConvert.DeserializeObject<EventObject>(json);

        if (evt.type == "Log")
        {
            MainThreadDispatcher.Enqueue(() => Log.Handle(evt));
        }
        else if (evt.type == "Udp")
        {
            MainThreadDispatcher.Enqueue(() => Input.HandleUdp(evt));
        }
        else if (evt.type == "Tcp")
        {
            MainThreadDispatcher.Enqueue(() => Server.HandleTcp(evt));
        }
        else if (evt.type == "State")
        {
            MainThreadDispatcher.Enqueue(() => Client.HandleState(evt));
        }
    }

    private T _ReadJson<T>(Func<IntPtr> nativeCall, JsonSerializerSettings settings = null)
    {
        IntPtr ptr = nativeCall();
        try
        {
            string json = Marshal.PtrToStringAnsi(ptr);
            if (settings == null)
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(json, settings);
            }
        }
        finally
        {
            SuduxuRaw.free(ptr);
        }
    }
}
