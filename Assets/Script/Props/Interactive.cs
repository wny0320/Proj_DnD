using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : MonoBehaviour
{
    private bool isInteractiving = false;
    private float distance = 3f;

    public List<Item> dropItemList;

    private void Start()
    {
        switch (transform.tag)
        {
            case "Chest":
                dropItemList = Manager.Inven.GetRandomItem(2, 5);
                break;
            case "Monster":
                dropItemList = Manager.Inven.GetRandomItem(1, 4);
                break;
        }
    }

    private void FixedUpdate()
    {
        if (!isInteractiving) return;

        if (Manager.Game.Player == null) return;
        if ((Manager.Game.Player.transform.position - transform.position).magnitude > distance || 
            Input.GetKeyDown(KeyCode.Tab))
        {
            isInteractiving = false;
            Manager.Inven.ConcealDropCanvas();
        }
    }

    public void InteractiveFunc()
    {
        isInteractiving = true;

        switch(transform.tag)
        {
            case "Item":
                ItemFunc();
                break;
            case "Door":
                DoorFunc();
                break;
            case "Chest":
                ChestFunc();
                break;
            case "Monster":
                MonsterFunc();
                break;
        }
    }

    private void ItemFunc()
    {
        Debug.Log("item");
        Item itemPickedUp = GetComponent<Item3D>().myItem;
        // 인벤토리에 아이템 넣기 성공
        if(Manager.Inven.AddItem(itemPickedUp, ItemBoxType.Inventory) == true)
        {
            Destroy(gameObject);
        }
    }
    private void DoorFunc()
    {
        Debug.Log("door");
    }
    private void ChestFunc()
    {
        Debug.Log("chest");
        Manager.Inven.RevealDropCanvas();
        Manager.Inven.RevealInvenCanvasByBt();
        Manager.Inven.ItemBoxReset(ItemBoxType.Drop);
        foreach (Item item in dropItemList)
            Manager.Inven.AddItem(item, ItemBoxType.Drop);
    }
    private void MonsterFunc()
    {
        Debug.Log("monster");
        Manager.Inven.RevealDropCanvas();
        Manager.Inven.RevealInvenCanvasByBt();
        Manager.Inven.ItemBoxReset(ItemBoxType.Drop);
        foreach (Item item in dropItemList)
            Manager.Inven.AddItem(item, ItemBoxType.Drop);
    }
}
