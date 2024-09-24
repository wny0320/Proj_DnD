using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InputManager
{
    public float mouseSpeed = 2f;

    public Action PlayerMove;
    public Action PlayerAttack;

    public void OnUpdate()
    {
        if (!Manager.Game.isPlayerAlive) return;

        PlayerMove?.Invoke();
        PlayerAttack?.Invoke();

        if (Manager.Game.isPlayerAttacking) return;
        SelectWeapon();
    }
    public void OnFixedUpdate()
    {

    }

    private void SelectWeapon()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            //¸Ç¼Õ
            Global.PlayerWeaponEquip(null);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Item weapon = CheckSlot(1, EquipPart.Weapon);

            Global.PlayerWeaponEquip(weapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Item weapon = CheckSlot(2, EquipPart.Weapon);

            Global.PlayerWeaponEquip(weapon);
        }
        //else if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    Item item = CheckSlot(1, ItemType.Consumable);
        //    if (item == null) return;
        //    Global.PlayerWeaponEquip(item);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    Item item = CheckSlot(1, ItemType.Consumable);
        //    if (item == null) return;
        //}
    }

    private Item CheckSlot(int n, EquipPart part)
    {
        return Manager.Inven.equipSlots[part.ToString() + n].slotItem;
    }

    private Item CheckSlot(int n, ItemType part)
    {
        return Manager.Inven.equipSlots[part.ToString() + n].slotItem;

    }
}
