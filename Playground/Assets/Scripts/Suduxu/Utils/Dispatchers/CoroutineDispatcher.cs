using UnityEngine;
using System.Collections;

public sealed class CoroutineDispatcher : MonoBehaviour
{
    private static CoroutineDispatcher _instance;

    public static CoroutineDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("CoroutineDispatcher");
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<CoroutineDispatcher>();
            }
            return _instance;
        }
    }

    public void Run(IEnumerator routine)
    {
        StartCoroutine(routine);
    }
}