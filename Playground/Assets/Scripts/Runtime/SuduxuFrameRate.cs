using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuduxuFrameRate : MonoBehaviour
{
    private Suduxu suduxu = new(1);

    private int frameCount = 0;
    private float elapsed = 0;

    private void Start()
    {
        suduxu.Launch();
        suduxu.RegisterCallbacks();

        suduxu.Log.OnLog += (log) =>
        {
            Debug.Log(log);
        };

        suduxu.Input.OnSensorData += (ref SensorDataRaw sensorData) =>
        {
            Debug.Log(sensorData);

            frameCount++;
        };
    }

    private void Update()
    {
        suduxu.Input.Tick(Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.A))
        {
            suduxu.Client.SetFrameRate(5);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            suduxu.Client.SendSensorData(true);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            suduxu.Client.Log(LogLevel.Warn, "Test Log");
        }

        if (elapsed > 1)
        {
            Debug.Log($"Rounds: {frameCount}");

            elapsed = 0;
            frameCount = 0;
        }

        elapsed += Time.deltaTime;
    }

    private void OnDestroy()
    {
        suduxu.Stop();
    }
}
