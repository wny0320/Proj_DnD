using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : MonoBehaviour
{
    private bool isInteractiving = false;
    private bool isOpened = false;
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
            dropItemList.Clear();
            dropItemList = Manager.Inven.GetBoxItems(ItemBoxType.Drop);
            isInteractiving = false;
            Manager.Inven.ConcealDropCanvas();
        }
    }

    public void InteractiveFunc()
    {
        switch(transform.tag)
        {
            case "Item":
                ItemFunc();
                break;
            case "Door":
                DoorFunc();
                break;
            case "Chest":
                isInteractiving = true;
                ChestFunc();
                break;
            case "Monster":
                isInteractiving = true;
                MonsterFunc();
                break;
            case "Torch":
                TorchFunc();
                break;
        }
    }

    private void ItemFunc()
    {
        Debug.Log("item");
        Item itemPickedUp = GetComponent<Item3D>().myItem;
        // 인벤토리에 아이템 넣기 성공
        if(Manager.Inven.AddItem(itemPickedUp, ItemBoxType.Inventory) != null)
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
        if (!isOpened)
        {
            GetComponent<Animation>().Play();
            isOpened = true;
        }

        Cursor.lockState = CursorLockMode.None;
        Manager.Inven.RevealDropCanvas();
        Manager.Inven.RevealInvenCanvasByBt();
        Manager.Inven.ItemBoxReset(ItemBoxType.Drop);
        foreach (Item item in dropItemList)
            Manager.Inven.AddItem(item, ItemBoxType.Drop);
    }
    private void MonsterFunc()
    {
        Debug.Log("monster");
        Cursor.lockState = CursorLockMode.None;
        Manager.Inven.RevealDropCanvas();
        Manager.Inven.RevealInvenCanvasByBt();
        Manager.Inven.ItemBoxReset(ItemBoxType.Drop);
        foreach (Item item in dropItemList)
            Manager.Inven.AddItem(item, ItemBoxType.Drop);
    }

    private void TorchFunc()
    {
        Debug.Log("torch");
        gameObject.GetComponent<Torch>().LightChange();
    }
}
