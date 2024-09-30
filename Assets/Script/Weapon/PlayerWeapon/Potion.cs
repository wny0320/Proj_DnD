using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : Weapon
{
    public Stat stat = null;
    bool isDrink = false;
    int heal = 25;

    public override void AttackStart(int level)
    {
        isDrink = true;
    }

    public override void AttackEnd()
    {
        if (isDrink)
        {
            PotionFunc();
        }
    }

    private void PotionFunc()
    {
        if (Manager.Game.isPlayerAlive)
        {
            stat.Hp = Mathf.Min(stat.Hp + heal, stat.MaxHp);
            Manager.Inven.DeleteInvenItem
                (Manager.Inven.equipSlots[ItemType.Consumable.ToString() + Manager.Input.currentUtilitySlot], ItemBoxType.Equip);
            Manager.Input.currentUtilitySlot = -1;

            Global.PlayerWeaponEquip(null);

            Destroy(gameObject);
        }
    }
}