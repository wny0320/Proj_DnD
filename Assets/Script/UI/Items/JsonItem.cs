using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonItem
{
    public string itemName;
    public int itemIndex;
    public string itemText;
    public int itemPrice;
    public ItemRarity itemRarity;
    public JsonStat itemStat;
    public ItemType itemType;
    public ItemSize itemSize;
    public EquipPart equipPart;
    public WeaponType weaponType;
    public bool equipStatSetFlag;
    public float[] randomRange = new float[2];
    public float duration;
    public float effect;

    public Item JsonItemToItem()
    {
        Item item = (Item)ScriptableObject.CreateInstance(typeof(Item));
        item.itemName = itemName;
        item.itemIndex = itemIndex;
        item.itemText = itemText;
        item.itemPrice = itemPrice;
        item.itemRarity = itemRarity;
        if(itemStat != null)
            item.itemStat = itemStat.JsonStatToStat();
        item.itemType = itemType;
        item.equipPart = equipPart;
        item.itemSize = itemSize;
        item.randomRange = randomRange;
        item.equipStatSetFlag = equipStatSetFlag;
        item.weaponType = weaponType;
        return item;
    }
}
