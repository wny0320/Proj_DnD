using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    // ���� �Ŵ��� - ���� �帧 ���� �ۼ�
    public GameObject Player;
    public bool isPlayerAlive = true;
    public bool isCursorLock = true;

    public void OnAwake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void CursorLock(bool cursurLock)
    {
        isCursorLock = cursurLock;

        if(cursurLock)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }
}
