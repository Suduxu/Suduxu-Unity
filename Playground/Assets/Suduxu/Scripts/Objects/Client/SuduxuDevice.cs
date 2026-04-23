public class SuduxuDevice
{
    public ushort id;
    public string name;
    public string deviceModel;
    public Os os;
    public string osVersion;
    public string manufacturer;
    public string screenResolution;
    public string screenSize;
    public string deviceId;
    public sbyte batteryLevel;
    public ChargingStatus chargingStatus;
    public NetworkType networkType;
    public string appVersion;
    public string ip;
    public ushort port;
    public ushort requests;
    public HealthStatus healthStatus;

    public override string ToString()
    {
        return $"SuduxuDevice {{ " +
               $"Id={id}, " +
               $"Name='{name ?? "N/A"}', " +
               $"DeviceModel='{deviceModel ?? "N/A"}', " +
               $"OS={os}, " +
               $"OSVersion='{osVersion ?? "N/A"}', " +
               $"Manufacturer='{manufacturer ?? "N/A"}', " +
               $"ScreenResolution='{screenResolution ?? "N/A"}', " +
               $"ScreenSize='{screenSize ?? "N/A"}', " +
               $"DeviceId='{deviceId ?? "N/A"}', " +
               $"BatteryLevel={batteryLevel}%, " +
               $"ChargingStatus={chargingStatus}, " +
               $"NetworkType={networkType}, " +
               $"AppVersion='{appVersion ?? "N/A"}', " +
               $"IP='{ip ?? "N/A"}', " +
               $"Port={port}, " +
               $"Requests={requests}, " +
               $"HealthStatus={healthStatus} " +
               $"}}";
    }
}