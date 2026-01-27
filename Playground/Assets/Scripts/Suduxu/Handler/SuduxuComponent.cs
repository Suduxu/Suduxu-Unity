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
        Debug.Log("Tick called");

        suduxu.Input.Tick(Time.deltaTime);
    }
}