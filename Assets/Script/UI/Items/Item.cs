using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string itemName;
    public int itemImageNum;
    //public string itemText; // �� �κ��� ��� �ɰ� ����
    public Stat itemStat;
    public EquipPart equipPart;
    public ItemSize itemSize;
    public int itemMaxStack;
    public int itemStack;
}
