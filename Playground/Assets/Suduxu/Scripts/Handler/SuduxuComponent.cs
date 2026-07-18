using UnityEngine;

public class SuduxuComponent : MonoBehaviour
{
    [SerializeField] public bool launchOnStart;
    [SerializeField] public bool stopOnDestroy;

    [Injected] private Suduxu suduxu;

    private void Start()
    {
        if (launchOnStart && !suduxu.IsRunning())
        {
            suduxu.Launch();
        }

        suduxu.Log.OnLog += log => { Debug.Log(log); };

        suduxu.Server.OnClientConnected += id =>
        {
            suduxu.Client.For(id).PlaySound("Duxu");
        };
    }

    private void OnDestroy()
    {
        if (stopOnDestroy && suduxu.IsRunning())
        {
            suduxu.Stop();
        }
    }

    private void Update()
    {
        suduxu.Input.Tick(Time.deltaTime);
    }
}