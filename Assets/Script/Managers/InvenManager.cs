using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvenManager
{
    Item[,] itemSlot = new Item[5,8];
    private const string INVENTORY_PATH = "Canvas/UIPanel/ItemArea/Content";
    Transform inventoryParent;

    public void OnStart()
    {

    }
}
