using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string itemName;
    public int itemIndex;
    public string itemText;
    public ItemRarity itemRarity;
    public Stat itemStat;
    public ItemType itemType;
    public EquipPart equipPart;
    public ItemSize itemSize;
    public bool randomStatFlag;
    public float[] randomRange = new float[2];
    //attack, defense, movespeed
    public Item ItemDeepCopy()
    {
        Item item = (Item)CreateInstance(typeof(Item));
        item.itemName = itemName;
        item.itemIndex = itemIndex;
        item.itemText = itemText;
        item.itemRarity = itemRarity;
        item.itemStat = itemStat.StatDeepCopy();
        item.itemType = itemType;
        item.equipPart = equipPart;
        item.itemSize = itemSize;
        item.randomRange = randomRange;
        item.randomStatFlag = randomStatFlag;
        return item;
    }

    public void ItemRandomStat()
    {
        itemStat.Attack += Random.Range(0, (int)randomRange[0]);
        itemStat.Defense += Random.Range(0, (int)randomRange[1]);
    }
}
