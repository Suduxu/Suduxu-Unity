using System;

public class SuduxuLog
{
    public event Action<Log> OnLog;

    public void Handle(EventObject evt)
    {
        OnLog?.Invoke(evt.value.ToObject<Log>());
    }
}