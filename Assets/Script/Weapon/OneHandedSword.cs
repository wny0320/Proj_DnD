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

    public override void AttackStart(int level)
    {
        base.AttackStart(level);

        switch(level)
        {
            case 0:
                Global.sfx.Play(Global.Sound.onehandedSwing1, transform.position);
                break;
            case 1:
                Global.sfx.Play(Global.Sound.onehandedSwing2, transform.position);
                break;
            case 2:
                Global.sfx.Play(Global.Sound.onehandedStab, transform.position);
                break;
        }
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
