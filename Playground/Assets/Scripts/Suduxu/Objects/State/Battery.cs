using Newtonsoft.Json;

public class Battery
{
    public sbyte level;

    [JsonProperty("charging_status")]
    public ChargingStatus chargingStatus;

    public Battery(sbyte level, ChargingStatus chargingStatus)
    {
        this.level = level;
        this.chargingStatus = chargingStatus;
    }
}