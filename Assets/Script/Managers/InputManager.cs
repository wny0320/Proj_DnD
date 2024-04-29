using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    public float mouseSpeed = 2f;

    public Action PlayerMove;
    public Action CameraMove;

    public void OnUpdate()
    {
        CameraMove.Invoke();
    }
    public void OnFixedUpdate()
    {
        PlayerMove.Invoke();
    }
}
