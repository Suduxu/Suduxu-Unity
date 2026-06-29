using System;

/// <summary>
/// Central class used for logs coming from the Suduxu-ABI-Bridge.
/// </summary>
public class SuduxuLog
{
    /// <summary>
    /// Event triggered when a log message is received from the Suduxu ABI bridge.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This event is raised whenever the underlying system publishes a log entry via the internal event bus.
    /// The log payload originates from the native layer and is deserialized into a <c>Log</c> object before emission.
    /// </para>
    /// <para>
    /// The event is not filtered by client ID; it represents global system-level log output.
    /// </para>
    /// <para>
    /// Parameter:
    /// </para>
    /// <para>
    /// - <c>log</c>: The deserialized log message received from the Suduxu ABI bridge.
    /// </para>
    /// </remarks>
    public event Action<Log> OnLog;

    public void Handle(EventObject evt)
    {
        OnLog?.Invoke(evt.value.ToObject<Log>());
    }
}