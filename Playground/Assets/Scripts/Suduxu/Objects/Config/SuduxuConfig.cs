using JetBrains.Annotations;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class SuduxuConfig
{
    public Server server;
    public Logging logging;
    public Security security;
    public FileSharing fileSharing;
    public Devices devices;
    public ScreenCapture screenCapture;
    public Sensors sensors;
    public Developer developer;
    public HealthCheck healthCheck;
}

public class Server
{
    public string address;
    public ushort? port;
    public ushort? tcpPort;
    public ushort? udpPort;
    public ushort? filePort;
    public ConnectionStrategy connectionStrategy;
    [CanBeNull] public List<string> list;
    public RateLimit rateLimit;
}

public class Logging
{
    public LogLevel debugLevel;
    [CanBeNull] public string logFile;
    public ulong? maxLogSize;
    public bool logToConsole;
}

public class Security
{
    public bool enabled;
    public uint? password;
}

public class FileSharing
{
    public bool enabled;
    [CanBeNull] public string sharedDirectory;
    [CanBeNull] public List<SharedFile> files;
    [CanBeNull] public List<string> initiallyLoaded;
}

public class Devices
{
    public bool initiallySendSensorData;
    public ushort? maxDevices;
    public List<Os> allowedDeviceTypes;
    public ushort initialFrameRate;
}

public class ScreenCapture
{
    public bool enabled;
    public bool captureOnServer;
    [CanBeNull] public string captureDirectory;
}

public class Sensors
{
    public bool accelerometer;
    public bool gyroscope;
    public bool magnetometer;
    public bool temperature;
    public bool humidity;
    public bool pressure;
    public bool light;
}

public class Developer
{
    public bool preferCli;
    public bool allowMockedSensors;
    public bool allowMockedButtons;
}

public class HealthCheck
{
    public HealthCheckConfig server;
    public HealthCheckConfig client;
}

public enum ConnectionStrategy
{
    Whitelist,
    Blacklist,
    Open
}

public class RateLimit
{
    public bool enabled;
    public ushort? maxTcpRequestsPerMinute;
}

public class SharedFile
{
    public string name;
    public string path;
    public SharedFileType type;
    [CanBeNull] public ThemeConstraints themeConstraints;
}

[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum SharedFileType
{
    Audio,
    [EnumMember(Value = "Lua-Theme")]
    LuaTheme,

    [EnumMember(Value = "XML-Theme")]
    XMLTheme
}

public class ThemeConstraints
{
    public ushort? minWidth;
    public ushort? maxWidth;
}

public class HealthCheckConfig
{
    public bool enabled;
    public ulong? intervalMs;
    public ulong? timeoutMs;
}