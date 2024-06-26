using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneHandedSword : Weapon
{
    private void Start()
    {
        col = GetComponent<Collider>();
        SetGlobalWeapon();
        col.enabled = false;

        SetWeaponInfo();
    }

    public override void SetWeaponInfo()
    {
        //여기에 데이터에서 가져오기 해야됨
        //무기 변경/장착 시 호출할 함수
        //임시로
        damage = 100;
        speedDebuff = 10;

        SetGlobalWeapon();
    }
}
