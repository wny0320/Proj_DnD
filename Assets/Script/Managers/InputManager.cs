using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InputManager
{
    public float mouseSpeed = 2f;
    public int currentUtilitySlot = -1;
    public int currentWeaponSlot = -1;

    public Action PlayerMove;
    public Action PlayerAttack;

    public void OnUpdate()
    {
        if (Manager.Instance.sceneName != SceneName.DungeonScene) return;

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
            Manager.Inven.MainEquipUIAlpha();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (currentWeaponSlot == 0) return;
            Item weapon = CheckSlot(0, EquipPart.Weapon);
            Manager.Inven.MainEquipUIAlpha(0);

            Global.PlayerWeaponEquip(weapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (currentWeaponSlot == 1) return;
            Item weapon = CheckSlot(1, EquipPart.Weapon);
            Manager.Inven.MainEquipUIAlpha(1);

            Global.PlayerWeaponEquip(weapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Item item = CheckSlot(0, ItemType.Consumable);
            if (item == null) return;
            Manager.Inven.MainEquipUIAlpha(2);
            Global.PlayerWeaponEquip(item);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Item item = CheckSlot(1, ItemType.Consumable);
            if (item == null) return;
            Manager.Inven.MainEquipUIAlpha(3);
            Global.PlayerWeaponEquip(item);
        }
    }

    private Item CheckSlot(int n, EquipPart part)
    {
        currentUtilitySlot = -1;
        currentWeaponSlot = n;
        return Manager.Inven.equipSlots[part.ToString() + n].slotItem;
    }

    private Item CheckSlot(int n, ItemType part)
    {
        currentWeaponSlot = -1;
        currentUtilitySlot = n;
        return Manager.Inven.equipSlots[part.ToString() + n].slotItem;
    }
}
