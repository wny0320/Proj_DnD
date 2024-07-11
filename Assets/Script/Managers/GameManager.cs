using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    // 게임 매니저 - 게임 흐름 관련 작성
    public GameObject Player;
    public bool isPlayerAlive = true;

    public void OnAwake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
