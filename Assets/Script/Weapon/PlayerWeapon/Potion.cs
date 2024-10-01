using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : Weapon
{
    public Stat stat = null;
    float potionDuration = 0;
    float potionEffect = 0;
    string potionName = null;
    bool isDrink = false;

    const string POTION_RED_NAME = "PotionRed";
    const string POTION_BLUE_NAME = "PotionBlue";
    const string POTION_GREEN_NAME = "PotionGreen";

    public override void AttackStart(int level)
    {
        isDrink = true;
    }

    public override void AttackEnd()
    {
        if (isDrink)
        {
            StartCoroutine(PotionFunc());
        }
    }

    public override void SetWeaponInfo()
    {
        Item item = GetComponent<Item3D>().myItem;
        potionDuration = item.duration;
        potionEffect = item.effect;
        potionName = item.name.ToString();
        SetGlobalWeapon();
    }

    private IEnumerator PotionFunc()
    {
        float timer = 0f;
        if (Manager.Game.isPlayerAlive)
        {
                Manager.Inven.DeleteBoxItem
                    (Manager.Inven.equipSlots[ItemType.Consumable.ToString() + Manager.Input.currentUtilitySlot], ItemBoxType.Equip);
                Manager.Input.currentUtilitySlot = -1;

                Global.PlayerWeaponEquip(null);

                Destroy(gameObject);
        }
        if (potionName.Equals(POTION_RED_NAME))
        {
            while(timer > potionDuration)
            {
                yield return new WaitForSeconds(1f);
                float deltaTime = Time.deltaTime;
                timer += deltaTime;
                stat.Hp = Mathf.Min(stat.Hp + Mathf.RoundToInt(potionEffect * deltaTime), stat.MaxHp);
            }
        }
    }
}