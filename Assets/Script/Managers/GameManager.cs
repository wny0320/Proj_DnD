using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    // ���� �Ŵ��� - ���� �帧 ���� �ۼ�
    public GameObject Player;

    public void OnAwake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
