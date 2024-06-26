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
        //���⿡ �����Ϳ��� �������� �ؾߵ�
        //���� ����/���� �� ȣ���� �Լ�
        //�ӽ÷�
        damage = 100;
        speedDebuff = 10;

        SetGlobalWeapon();
    }
}
