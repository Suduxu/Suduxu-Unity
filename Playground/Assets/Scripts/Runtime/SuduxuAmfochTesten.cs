using UnityEngine;

public class SuduxuAmfochTesten : MonoBehaviour
{
    [Injected] private Suduxu suduxu;

    private ushort id = 9999;

    private void Start()
    {
        suduxu.Server.OnClientConnected += id =>
        {
            if (this.id == 9999)
            {
                this.id = id;
            }
        };

        suduxu.Input.OnButtonInput += (_, input) =>
        {
            Debug.Log($"Input parsed: {input}");
        };
    }

    private void Update()
    {
        if (suduxu.Input.For(id).GetButtonDown(ButtonInputType.A))
        {
            suduxu.Client.For(id).Vibrate(1500, VibrationStrength.Heavy, VibrationType.Custom);
        }

        if (suduxu.Input.For(id).GetButton(ButtonInputType.A))
        {
            Debug.Log("A Down");
        }
    }
}
