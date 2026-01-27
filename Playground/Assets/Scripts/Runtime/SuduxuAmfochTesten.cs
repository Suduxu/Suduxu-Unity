using System;
using UnityEngine;

public class SuduxuAmfochTesten : MonoBehaviour
{
    [Injected] private Suduxu suduxu;

    private ushort id = 9999;

    private void Start()
    {
        suduxu.Server.OnClientConnected += id =>
        {
            this.id = id;
        };
    }

    private void Update()
    {
        if (suduxu.Input.For(id).GetButtonDown(ButtonInputType.A))
        {

        }

        if (suduxu.Input.For(id).GetButton(ButtonInputType.A))
        {
            Debug.Log("A Down");
        }
    }
}
