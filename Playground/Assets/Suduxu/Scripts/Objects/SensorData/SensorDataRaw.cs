using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct SensorDataRaw
{
    public ushort id;

    public byte mocked;

    public float ax;
    public float ay;
    public float az;

    public float gx;
    public float gy;
    public float gz;
    public float gw;

    public float mx;
    public float my;
    public float mz;

    public float temperature;
    public float humidity;
    public float pressure;
    public float light;

    public SensorDataRaw(ushort id, byte mocked, float ax, float ay, float az, float gx, float gy, float gz, float gw, float mx, float my, float mz, float temperature, float humidity, float pressure, float light)
    {
        this.id = id;
        this.mocked = mocked;
        this.ax = ax;
        this.ay = ay;
        this.az = az;
        this.gx = gx;
        this.gy = gy;
        this.gz = gz;
        this.gw = gw;
        this.mx = mx;
        this.my = my;
        this.mz = mz;
        this.temperature = temperature;
        this.humidity = humidity;
        this.pressure = pressure;
        this.light = light;
    }

    public SensorData toSensorData()
    {
        return new SensorData(id, mocked == 1, ax, ay, az, gx, gy, gz, gw, mx, my, mz, temperature, humidity, pressure, light);
    }
}