using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InputManager
{
    public float mouseSpeed = 2f;
    public int currentUtilitySlot = 0;
    public int currentWeaponSlot = -1;

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
            currentUtilitySlot = -1;
            Global.PlayerWeaponEquip(null);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (currentWeaponSlot == 0) return;
            Item weapon = CheckSlot(0, EquipPart.Weapon);

            Global.PlayerWeaponEquip(weapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (currentWeaponSlot == 1) return;
            Item weapon = CheckSlot(1, EquipPart.Weapon);

            Global.PlayerWeaponEquip(weapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Item item = CheckSlot(0, ItemType.Consumable);
            if (item == null) return;
            Global.PlayerWeaponEquip(item);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Item item = CheckSlot(1, ItemType.Consumable);
            if (item == null) return;
            Global.PlayerWeaponEquip(item);
        }
    }

    private Item CheckSlot(int n, EquipPart part)
    {
        currentWeaponSlot = n;
        return Manager.Inven.equipSlots[part.ToString() + n].slotItem;
    }

    private Item CheckSlot(int n, ItemType part)
    {
        currentUtilitySlot = n;
        return Manager.Inven.equipSlots[part.ToString() + n].slotItem;
    }
}
