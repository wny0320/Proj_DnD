using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : Weapon
{
    bool isDrink = false;

    public override void AttackStart(int level)
    {
        isDrink = true;
    }

    public override void AttackEnd()
    {
        if(isDrink)
        {
            Global.PlayerWeaponEquip(null);
            Destroy(gameObject);
        }
    }

    //�ؾߵ� �� - ���� �÷��̾� ü�� ä���
    //�κ��� ���� ����
}
