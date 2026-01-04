using UnityEngine;
using System;
using System.Collections.Generic;

public class MainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> actions = new Queue<Action>();
    private static MainThreadDispatcher instance;

    public static MainThreadDispatcher Instance => instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (instance != null) return;

        var go = new GameObject("Main Thread Dispatcher");
        instance = go.AddComponent<MainThreadDispatcher>();
        DontDestroyOnLoad(go);
    }

    public static void Enqueue(Action action)
    {
        if (action == null) return;

        lock (actions)
        {
            actions.Enqueue(action);
        }
    }

    private void Update()
    {
        lock (actions)
        {
            while (actions.Count > 0)
            {
                actions.Dequeue()?.Invoke();
            }
        }
    }
}