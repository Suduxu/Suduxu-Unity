using System;

public class Log
{
    public LogLevel level;
    public string message;
    public ulong timestamp;

    public override string ToString()
    {
        string color = level switch
        {
            LogLevel.Error => "red",
            LogLevel.Warning => "yellow",
            LogLevel.Info => "white",
            LogLevel.Debug => "grey",
            _ => "white"
        };

        return $"<color={color}>[{level}] {message}</color>";
    }
}