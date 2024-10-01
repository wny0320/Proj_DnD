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
        potionName = item.itemName;

        SetGlobalWeapon();
    }

    private IEnumerator PotionFunc()
    {
        float timer = 0f;
        if (Manager.Game.isPlayerAlive)
        {
            //사용 후 인벤에서 삭제
            Manager.Inven.DeleteBoxItem
                (Manager.Inven.equipSlots[ItemType.Consumable.ToString() + Manager.Input.currentUtilitySlot], ItemBoxType.Equip);
            Manager.Input.currentUtilitySlot = -1;

            //사용 후 빈손으로
            Global.PlayerWeapon = null;
            Global.PlayerWeaponEquip(null);

            Destroy(GetComponent<Collider>());
            Destroy(GetComponent<MeshRenderer>());
            Destroy(GetComponent<MeshFilter>());
            Destroy(GetComponent<Interactive>());
        }

        if (potionName.Equals(POTION_RED_NAME))
        {
            while (timer < potionDuration)
            {
                timer += 1;
                stat.Hp = Mathf.Min(stat.Hp + Mathf.RoundToInt(potionEffect), stat.MaxHp);
                yield return new WaitForSeconds(1f);
            }
        }
        if (potionName.Equals(POTION_BLUE_NAME))
        {
            stat.MoveSpeed += potionEffect;
            while (timer < potionDuration)
            {
                timer += 1;
                yield return new WaitForSeconds(1f);
            }
            stat.MoveSpeed -= potionEffect;
        }
        if (potionName.Equals(POTION_GREEN_NAME))
        {
            stat.JumpForce += potionEffect;
            while (timer < potionDuration)
            {
                timer += 1;
                yield return new WaitForSeconds(1f);
            }
            stat.JumpForce -= potionEffect;
        }
        Destroy(gameObject);
    }
}