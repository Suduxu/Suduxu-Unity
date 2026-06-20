using UnityEngine;

public class PhoneInstance : MonoBehaviour
{
    [Injected] private Suduxu suduxu;

    [SerializeField] private ushort id = SuduxuId.BroadcastId;

    [SerializeField] private GameObject phone;

    private void Start()
    {
        suduxu.Input.For(id).OnSensorData += (_, data) =>
        {
            var androidQuat = new Quaternion(
                data.gx,
                data.gy,
                data.gz,
                data.gw
            );

            var unityQuat = new Quaternion(
                -androidQuat.x,
                -androidQuat.z,
                -androidQuat.y,
                androidQuat.w
            );

            phone.transform.rotation = unityQuat;
        };
    }

    private void Update()
    {
        if (suduxu.Input.For(1).GetButtonDown(ButtonInputType.A))
        {
            Debug.Log("A Pressed");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            suduxu.Client.For(id).SendSensorData(true);
        }
    }
}