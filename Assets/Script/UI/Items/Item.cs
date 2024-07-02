using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string itemName;
    public int itemImageNum;
    //public string itemText; // 이 부분은 없어도 될거 같음
    public Stat itemStat;
    public EquipPart equipPart;
    public ItemSize itemSize;
    public int itemMaxStack;
    public int itemStack;
}
