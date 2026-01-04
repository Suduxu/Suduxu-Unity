using UnityEngine;

public class SuduxuEventTest : MonoBehaviour
{
    [Injected] private Suduxu suduxu;

    public GameObject prefab;

    private void Start()
    {
        suduxu.Server.OnClientConnected += id =>
        {
            Debug.Log("Call");

            Instantiate(prefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        };
    }
}
