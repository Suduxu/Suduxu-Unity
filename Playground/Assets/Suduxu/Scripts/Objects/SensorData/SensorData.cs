using UnityEngine;

public struct SensorData
{
    public ushort Id { get; private set; }
    public bool Mocked { get; private set; }
    public float Ax { get; private set; }
    public float Ay { get; private set; }
    public float Az { get; private set; }
    public float Gx { get; private set; }
    public float Gy { get; private set; }
    public float Gz { get; private set; }
    public float Gw { get; private set; }
    public float Mx { get; private set; }
    public float My { get; private set; }
    public float Mz { get; private set; }
    public float Temperature { get; private set; }
    public float Humidity { get; private set; }
    public float Pressure { get; private set; }
    public float Light { get; private set; }

    public SensorData(ushort id, bool mocked, float ax, float ay, float az, float gx, float gy, float gz, float gw, float mx, float my, float mz, float temperature, float humidity, float pressure, float light)
    {
        Id = id;
        Mocked = mocked;
        Ax = ax;
        Ay = ay;
        Az = az;
        Gx = gx;
        Gy = gy;
        Gz = gz;
        Gw = gw;
        Mx = mx;
        My = my;
        Mz = mz;
        Temperature = temperature;
        Humidity = humidity;
        Pressure = pressure;
        Light = light;
    }

    public Vector3SensorData Vectorize()
    {
        return new Vector3SensorData(new Vector3(Ax, Ay, Az), new Vector3(Gx, Gy, Gz), new Vector3(Mx, My, Mz));
    }

    public override string ToString()
    {
        return $"Accel: (X={Ax}, Y={Ay}, Z={Az}), " +
               $"Gyro: (X={Gx}, Y={Gy}, Z={Gz}, W={Gw}), " +
               $"Mag: (X={Mx}, Y={My}, Z={Mz})";
    }

}

public struct Vector3SensorData
{
    public Vector3 Accelerometer { get; private set; }
    public Vector3 Gyroscope { get; private set; }
    public Vector3 Magnetometer { get; private set; }

    public Vector3SensorData(Vector3 accelerometer, Vector3 gyroscope, Vector3 magnetometer)
    {
        Accelerometer = accelerometer;
        Gyroscope = gyroscope;
        Magnetometer = magnetometer;
    }
}