using UnityEngine;

public class SuduxuLauncher : MonoBehaviour
{
    private Suduxu suduxu = new Suduxu(1);
    private bool isSending = false;

    private void Start()
    {
        // Launch Suduxu server
        suduxu.Launch();
        suduxu.RegisterCallbacks();

        // ===== Subscribe to events =====
        suduxu.Log.OnLog += HandleLog;

        suduxu.Server.OnTcpStart += () => Debug.Log("TCP Server Started");
        suduxu.Server.OnTcpStop += () => Debug.Log("TCP Server Stopped");

        suduxu.Server.OnClientConnected += id => Debug.Log($"Client connected: {id}");
        suduxu.Server.OnClientDisconnected += id => Debug.Log($"Client disconnected: {id}");

        suduxu.Server.OnError += message => Debug.Log($"Server Error: {message}");
        suduxu.Server.OnIllegalSuduxuMethod += payload => Debug.Log($"Illegal payload: {payload}");

        suduxu.Input.OnButtonInput += (id, input) => Debug.Log($"Client {id} Button Input: {input}");
        suduxu.Input.OnJoystickData += (id, data) => Debug.Log($"Client {id} Joystick Data: {data}");
        suduxu.Input.OnSensorData += OnSensorData;

        suduxu.Input.OnUdpStart += () => Debug.Log("UDP Server Started");
        suduxu.Input.OnUdpStop += () => Debug.Log("UDP Server Stopped");
    }

    private void OnDestroy()
    {
        suduxu.Stop();
    }

    // ===== Event Handlers =====
    private void OnSensorData(ref SensorDataRaw data)
    {
        Debug.Log($"Sensor azimuth: {data.az}");
    }

    private void HandleLog(Log log)
    {
        switch (log.level)
        {
            case LogLevel.Debug:
            case LogLevel.Info:
                Debug.Log(log);
                break;
            case LogLevel.Warn:
                Debug.LogWarning(log);
                break;
            case LogLevel.Error:
                Debug.LogError(log);
                break;
        }
    }

    // ===== Unity Update Loop =====
    private void Update()
    {
        // Tick Input subsystem
        suduxu.Input.Tick(Time.deltaTime);

        // List all connected clients
        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (var client in suduxu.FindAllClients())
                Debug.Log(client);
        }

        // Disconnect all clients
        if (Input.GetKeyDown(KeyCode.D))
        {
            suduxu.DisconnectAll();
            Debug.Log("All clients disconnected");
        }

        // Input checks using the new Input subsystem
        if (suduxu.Input.GetButtonDown(ButtonInputType.A))
            Debug.Log("A Down");

        if (suduxu.Input.GetButton(ButtonInputType.A))
            Debug.Log("A Pressed or Down");

        if (suduxu.Input.GetButtonUp(ButtonInputType.A))
            Debug.Log("A Up");

        // Vibrate and play sound using Client.For
        if (Input.GetKeyDown(KeyCode.V))
            suduxu.Client.For(suduxu.Input.Id)
                         .Vibrate(500, VibrationStrength.Heavy, VibrationType.Custom);

        if (Input.GetKeyDown(KeyCode.P))
            suduxu.Client.For(suduxu.Input.Id)
                         .PlaySound("Sample");

        // Toggle sending sensor data
        if (Input.GetKeyDown(KeyCode.S))
        {
            isSending = !isSending;
            suduxu.Client.For(suduxu.Input.Id)
                         .SendSensorData(isSending);
        }
    }
}
