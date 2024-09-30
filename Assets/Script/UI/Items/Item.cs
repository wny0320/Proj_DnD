using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string itemName;
    public int itemIndex;
    public string itemText;
    public int itemPrice;
    public ItemRarity itemRarity;
    public Stat itemStat;
    public ItemType itemType;
    public EquipPart equipPart;
    public ItemSize itemSize;
    public WeaponType weaponType;
    public bool equipStatSetFlag;
    public float[] randomRange = new float[2];
    //attack, defense, price
    public Item ItemDeepCopy()
    {
        Item item = (Item)CreateInstance(typeof(Item));
        item.itemName = itemName;
        item.itemIndex = itemIndex;
        item.itemText = itemText;
        item.itemPrice = itemPrice;
        if (itemRarity != ItemRarity.Non)
            item.itemRarity = itemRarity;
        else
        {
            item.itemRarity = ItemRarirySet();
            item.itemPrice = item.ItemPriceSet();
        }
        if (itemStat != null)
            item.itemStat = itemStat.StatDeepCopy();
        item.itemType = itemType;
        item.equipPart = equipPart;
        item.itemSize = itemSize;
        item.randomRange = randomRange;
        item.equipStatSetFlag = equipStatSetFlag;
        item.weaponType = weaponType;
        return item;
    }

    public void EquipStatSet()
    {
        if (equipPart == EquipPart.Weapon)
        {
            switch (itemRarity)
            {
                case ItemRarity.Poor:
                    itemStat.Attack += 2;
                    break;
                case ItemRarity.Common:
                    itemStat.Attack += 4;
                    break;
                case ItemRarity.Rare:
                    itemStat.Attack += 6;
                    break;
                case ItemRarity.Epic:
                    itemStat.Attack += 8;
                    break;
                case ItemRarity.Legendary:
                    itemStat.Attack += 10;
                    break;
            }
        }
        else
        {
            switch (itemRarity)
            {
                case ItemRarity.Poor:
                    itemStat.Defense += 1;
                    break;
                case ItemRarity.Common:
                    itemStat.Defense += 2;
                    break;
                case ItemRarity.Rare:
                    itemStat.Defense += 3;
                    break;
                case ItemRarity.Epic:
                    itemStat.Defense += 4;
                    break;
                case ItemRarity.Legendary:
                    itemStat.Defense += 5;
                    break;
            }
        }
        itemStat.Attack += Random.Range(0, (int)randomRange[0]);
        itemStat.Defense += Random.Range(0, (int)randomRange[1]);
    }

    public ItemRarity ItemRarirySet()
    {
        ItemRarity rarity = ItemRarity.Non;
        int itemChance = Random.Range(0, 100);
        if (itemChance < 1) // 1%
            rarity = ItemRarity.Legendary;
        else if (itemChance < 5) // 4%
            rarity = ItemRarity.Epic;
        else if (itemChance < 15) // 10%
            rarity = ItemRarity.Rare;
        else if (itemChance < 40) // 25%
            rarity = ItemRarity.Common;
        else if (itemChance < 100) // 60%
            rarity = ItemRarity.Poor;
        return rarity;
    }
    public int ItemPriceSet()
    {
        int price = 0;
        switch (itemRarity)
        {
            case ItemRarity.Poor:
                price = Mathf.RoundToInt(itemPrice * 1.2f);
                break;
            case ItemRarity.Common:
                price = Mathf.RoundToInt(itemPrice * 1.5f);
                break;
            case ItemRarity.Rare:
                price = Mathf.RoundToInt(itemPrice * 2.0f);
                break;
            case ItemRarity.Epic:
                price = Mathf.RoundToInt(itemPrice * 2.5f);
                break;
            case ItemRarity.Legendary:
                price = Mathf.RoundToInt(itemPrice * 3f);
                break;
        }
        return price;
    }
}
