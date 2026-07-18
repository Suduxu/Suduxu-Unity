using Newtonsoft.Json;

public class SensorTransmissionRate
{
    [JsonProperty("rate")]
    public ushort rate;

    public SensorTransmissionRate(ushort rate)
    {
        this.rate = rate;
    }
}