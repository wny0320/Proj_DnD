using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    //델리게이트 연결로 인풋매니저 사용
    KeyCode ChangeWeaponQ = KeyCode.Q;
    KeyCode ChangeWeaponE = KeyCode.E;
    KeyCode Up = KeyCode.W;
    KeyCode Left = KeyCode.A;
    KeyCode Down = KeyCode.S;
    KeyCode Right = KeyCode.D;
    KeyCode Dodge = KeyCode.Space;

    //밑의 대리자를 Global에 함수를 둘지 이곳에 둘지
    public Action PlayerMove = null;

    public void OnUpdate()
    {
        if (!Input.anyKey) return;

        //플레이어 이동 함수 정의 후 연결 해줘야됨
        PlayerMove.Invoke();
    }
}
