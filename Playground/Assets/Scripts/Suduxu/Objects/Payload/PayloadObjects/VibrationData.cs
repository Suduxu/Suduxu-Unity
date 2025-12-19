using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class VibrationData
{
    [JsonProperty("duration_ms")]
    public long durationMs;

    [JsonConverter(typeof(StringEnumConverter))]
    public VibrationStrength strength;

    [JsonConverter(typeof(StringEnumConverter))]
    public VibrationType type;

    public VibrationData(long durationMs, VibrationStrength strength, VibrationType type)
    {
        this.durationMs = durationMs;
        this.strength = strength;
        this.type = type;
    }
}

public enum VibrationStrength
{
    Light,
    Medium,
    Heavy
}

public enum VibrationType
{
    Impact,
    Notification,
    Selection,
    Custom
}