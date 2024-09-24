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

    //해야될 것 - 실제 플레이어 체력 채우기
    //인벤의 포션 삭제
}
