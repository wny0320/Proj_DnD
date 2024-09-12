using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    public float mouseSpeed = 2f;

    public Action PlayerMove;
    public Action PlayerAttack;

    public void OnUpdate()
    {
        PlayerMove.Invoke();
        PlayerAttack.Invoke();
    }
    public void OnFixedUpdate()
    {

    }
}
