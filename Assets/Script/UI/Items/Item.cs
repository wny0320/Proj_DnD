using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string itemName;
    public int itemImageNum;
    public string itemText;
    public ItemRarity itemRarity;
    public Stat itemStat;
    public ItemType itemType;
    public EquipPart equipPart;
    public ItemSize itemSize;
    public int itemMaxStack;
    [HideInInspector]
    public int itemStack;
}
