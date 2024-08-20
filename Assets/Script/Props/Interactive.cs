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
