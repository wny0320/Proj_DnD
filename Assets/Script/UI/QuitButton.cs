using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public GameObject confirmBackGround;
    private void Awake()
    {
        Application.quitting += QuitButtonFunc; // °­Á¾½Ã
        confirmBackGround.SetActive(false);
    }
    public void QuitButtonFunc()
    {
        Debug.Log("Quit");
        if (Manager.Instance.GetNowScene().name == SceneName.DungeonScene.ToString())
        {
            Manager.Inven.ItemBoxReset(ItemBoxType.Inventory);
            Manager.Inven.ResetEquipSlots();
        }
        Application.Quit();
    }
    public void QuitConfirmFunc()
    {
        Debug.Log("Quit Confirm");
        if (Manager.Instance.GetNowScene().name == SceneName.DungeonScene.ToString())
            confirmBackGround.SetActive(true);
        else
            QuitButtonFunc();
    }
    public void QuitConfirmDenyFunc()
    {
        Debug.Log("Quit Confirm Deny");
        confirmBackGround.SetActive(false);
    }
}
