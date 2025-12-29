using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum LogLevel
{
    Debug,
    Info,
    Warn,
    Error,
}
