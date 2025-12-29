using Newtonsoft.Json;

public class LogObject
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string title;
    public string message;
    public LogLevel level;

    public LogObject(LogLevel level, string message, string title)
    {
        this.title = title;
        this.message = message;
        this.level = level;
    }

    public LogObject(LogLevel level, string message)
    {
        this.message = message;
        this.level = level;
    }
}