using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraController : MonoBehaviour
{
    void LateUpdate()
    {
        if (Manager.Game.Player == null) return;

        transform.position = Manager.Game.Player.transform.position + Vector3.up * 50;
    }
}
