using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Central class for using Suduxu functionality
/// </summary>
[Injectable]
public class Suduxu : MonoBehaviour
{
    public static Suduxu Instance { get; private set; }

    [SerializeField]
    private ushort defaultId = 0;

    [SerializeField] 
    public RawImage qrUiElement;

    public ushort DefaultId => defaultId;

    /// <summary>
    /// Object used for retrieving input from specified clients
    /// </summary>
    public SuduxuInput Input { get; private set; }

    /// <summary>
    /// Object used for interacting with clients and retrieving client data.
    /// </summary>
    public SuduxuClient Client { get; private set; }

    /// <summary>
    /// Object used for managing server events.
    /// </summary>
    public SuduxuServer Server { get; private set; }
    
    /// <summary>
    /// Object used for logs coming from the Suduxu-ABI-Bridge.
    /// </summary>
    public SuduxuLog Log { get; private set; }

    /// <summary>
    /// Object used for managing QR-Code based connection.
    /// </summary>
    public SuduxuQr Qr { get; private set; }

    private static SuduxuRaw.EventCallback _eventCallback;
    private static SuduxuRaw.SensorEventCallback _sensorCallback;

    JsonSerializerSettings settings = new()
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

    /// <summary>
    /// Returns the configuration from <c>suduxu.json</c>
    /// Before accessing the configuration, a timeout for 200-500ms should be installed to ensure, that the configuration is loaded.
    /// </summary>
    /// <returns>
    /// The <c>suduxu.json</c> configuration.
    /// </returns>
    public SuduxuConfig Config {
        get
        {
            return SuduxuConfig.Instance;
        }
    }

    /// <summary>
    /// Returns the addresses on which the servers run.
    /// Before accessing the addresses, a timeout for 200-500ms should be installed to ensure, that the configuration is loaded.
    /// </summary>
    /// <returns>
    /// The addresses on which the servers run.
    /// </returns>
    public AddressObject Addresses => GetAddresses();

    /// <summary>
    /// Returns the password which is needed to authenticate to the server (if enabled).
    /// Before accessing the password, a timeout for 200-500ms should be installed to ensure, that the configuration is loaded.
    /// </summary>
    /// <returns>
    /// A nullable unsigned integer, representing an OTP-Code for Authentication (<c>null</c> if not enabled)
    /// </returns>
    public uint? Password => Config.security.password;

    /// <summary>
    /// Returns the version of the <c>suduxu.[dll/so/dylib]</c>
    /// </summary>
    /// <returns>
    /// A string representing the version of the ABI-Bridge.
    /// </returns>
    public string DllVersion()
    {
        IntPtr ptr = SuduxuRaw.version();
        string version;

        try
        {
            version = Marshal.PtrToStringAnsi(ptr);
        } finally
        {
            SuduxuRaw.suduxu_free(ptr);
        }

        return version;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Input = new SuduxuInput(defaultId);
        Client = new SuduxuClient(defaultId);
        Server = new SuduxuServer();
        Log = new SuduxuLog();
        Qr = new SuduxuQr(qrUiElement);
    }

    private void Start()
    {
        StartCoroutine(RefreshQr(100));

        StartCoroutine(LoadConfig(500));
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

    /// <summary>
    /// Launches the Suduxu runtime.
    /// </summary>
    /// <remarks>
    /// Registers all required callbacks and starts the runtime on a background
    /// thread.
    /// </remarks>
    public void Launch()
    {
        RegisterCallbacks();

        SuduxuRaw.serverThread = new Thread(() =>
        {
            SuduxuRaw.start_suduxu().ToFFIStatus().ThrowIfException();
        })
        {
            IsBackground = true
        };

        SuduxuRaw.serverThread.Start();
    }

    /// <summary>
    /// Stops the Suduxu runtime.
    /// </summary>
    /// <remarks>
    /// This method blocks until the runtime thread has terminated.
    /// </remarks>
    public void Stop()
    {
        SuduxuRaw.stop_suduxu().ToFFIStatus().ThrowIfException();

        if (SuduxuRaw.serverThread != null && SuduxuRaw.serverThread.IsAlive)
        {
            SuduxuRaw.serverThread.Join();
            SuduxuRaw.serverThread = null;
        }
    }

    /// <summary>
    /// Returns whether Suduxu is currently running.
    /// </summary>
    /// <returns>
    /// A boolean representing whether Suduxu is running.
    /// </returns>
    public bool IsRunning()
    {
        return SuduxuRaw.is_running();
    }

    /// <summary>
    /// Changes the default target client used by
    /// <see cref="Input"/> and <see cref="Client"/>.
    /// </summary>
    /// <remarks>
    /// Subsequent operations performed through <see cref="Input"/> and
    /// <see cref="Client"/> will target the specified client until changed again.
    /// </remarks>
    public void UseClient(ushort id)
    {
        Input.For(id);
        Client.For(id);
    }

    /// <summary>
    /// Returns metadata for all currently connected clients.
    /// </summary>
    /// <returns>
    /// A list containing every connected client.
    /// </returns>
    public List<SuduxuDevice> FindAllClients()
    {
        return _ReadJson<List<SuduxuDevice>>(SuduxuRaw.find_all_clients);
    }

    /// <summary>
    /// Returns the client of specified ID, null if absent
    /// </summary>
    /// <param name="id">ID of the client which's data should be obtained.</param>
    /// <returns>Metadata of the connected client with specified ID (null if absent)</returns>
    public SuduxuDevice FindClientById(ushort id)
    {
        SuduxuDevice device =
            _ReadJson<SuduxuDevice>(() => SuduxuRaw.find_client_by_id(id));

        return device;
    }

    /// <summary>
    /// Disconnects all clients
    /// </summary>
    public void DisconnectAll()
    {
        SuduxuRaw.disconnect_all().ToFFIStatus().ThrowIfException();
    }

    /// <summary>
    /// Disconnects client with specified ID.
    /// </summary>
    /// <param name="id">Client with specified ID who should be disconnected.</param>
    public void DisconnectClient(ushort id)
    {
        SuduxuRaw.disconnect_client(id).ToFFIStatus().ThrowIfException();
    }

    private void _OnEvent(IntPtr ptr)
    {
        string json = Marshal.PtrToStringAnsi(ptr);
        SuduxuRaw.suduxu_free(ptr);

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
        else if (evt.type == "Health")
        {
            MainThreadDispatcher.Enqueue(() => Client.HandleHealth(evt));
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
            SuduxuRaw.suduxu_free(ptr);
        }
    }

    private IEnumerator RefreshQr(float millis)
    {
        yield return new WaitForSecondsRealtime(millis / 1000.0f);
        Qr.RefreshQrCode();
    }

    private IEnumerator LoadConfig(float millis)
    {
        yield return new WaitForSecondsRealtime(millis / 1000.0f);
        SuduxuConfig.Instance = GetConfig();
    }
}
