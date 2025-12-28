using Newtonsoft.Json;

public class FrameRate
{
    [JsonProperty("frame_rate")]
    public ushort frameRate;

    public FrameRate(ushort frameRate)
    {
        this.frameRate = frameRate;
    }
}