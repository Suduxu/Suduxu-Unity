using System;
using UnityEngine;

public class SuduxuEventTest : MonoBehaviour
{
    [Injected] private Suduxu suduxu;

    public GameObject prefab;

    private GameObject instance;

    private void Start()
    {
        suduxu.Server.OnClientConnected += id =>
        {
            Debug.Log("Call");

            instance = Instantiate(prefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        };

        
        suduxu.Client.OnBatteryChange += (id, battery) =>
        {
            Debug.Log(
                $"Client with {id} has {battery.level}% and is {(battery.chargingStatus != ChargingStatus.Charging ? "not" : "")} charging");
        };

        suduxu.Input.Broadcast().OnJoystickData += (id, data) =>
        {
            Debug.Log($"Client with id {id} submitted data: {data}");
        };

        suduxu.Input.For(1).OnSensorData += sensorData =>
        {
            var androidQuat = new Quaternion(
                sensorData.gx,
                sensorData.gy,
                sensorData.gz,
                sensorData.gw
            );

            var unityQuat = new Quaternion(
                -androidQuat.x,
                -androidQuat.z,
                -androidQuat.y,
                androidQuat.w
            );

            Debug.Log(sensorData);

            instance.transform.rotation = unityQuat;
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            suduxu.Client.For(1).SendSensorData(true);
        }
    }
}