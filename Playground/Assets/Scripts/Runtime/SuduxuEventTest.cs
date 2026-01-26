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
            instance = Instantiate(prefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        };

        suduxu.Input.For(1).OnSensorData += (id, sensorData)=>
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

            Debug.Log(instance);

            instance.transform.rotation = unityQuat;
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SuduxuRaw.debug_unity_path();
        }
    }
}