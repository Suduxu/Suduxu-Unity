using System;
using UnityEngine;

public class SuduxuAmfochTesten : MonoBehaviour
{
    [Injected] private Suduxu suduxu;

    private void Start()
    {
        suduxu.Server.OnClientConnected += id =>
        {
            Debug.Log(id);
        };
    }

    private void Update()
    {
        if (suduxu.Input.For(1).GetButtonDown(ButtonInputType.A))
        {
            Debug.Log("Hallo Luca");
        }
    }
}
