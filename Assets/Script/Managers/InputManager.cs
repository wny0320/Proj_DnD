using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    //��������Ʈ ����� ��ǲ�Ŵ��� ���
    KeyCode ChangeWeaponQ = KeyCode.Q;
    KeyCode ChangeWeaponE = KeyCode.E;
    KeyCode Up = KeyCode.W;
    KeyCode Left = KeyCode.A;
    KeyCode Down = KeyCode.S;
    KeyCode Right = KeyCode.D;
    KeyCode Dodge = KeyCode.Space;

    //���� �븮�ڸ� Global�� �Լ��� ���� �̰��� ����
    public Action PlayerMove = null;

    public void OnUpdate()
    {
        if (!Input.anyKey) return;

        //�÷��̾� �̵� �Լ� ���� �� ���� ����ߵ�
        PlayerMove.Invoke();
    }
}
