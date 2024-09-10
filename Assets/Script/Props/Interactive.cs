using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : MonoBehaviour
{
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
                ChestFunc();
                break;
        }
    }

    private void ItemFunc()
    {
        Debug.Log("item");
        Item itemPickedUp = GetComponent<Item3D>().myItem;
        // 인벤토리에 아이템 넣기 성공
        if(Manager.Inven.AddItem(itemPickedUp) == true)
        {
            Destroy(gameObject);
        }
        else
        {
            GetComponent<GameObject>().transform.position += new Vector3(0, 1f, 0);
        }
    }
    private void DoorFunc()
    {
        Debug.Log("door");
    }
    private void ChestFunc()
    {
        Debug.Log("chest");
    }
}
