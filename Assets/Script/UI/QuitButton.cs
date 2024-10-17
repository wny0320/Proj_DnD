using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public GameObject confirmBackGround;
    private bool quitFlag;
    private void Awake()
    {
        Application.quitting += QuitButtonFunc; // °­Á¾½Ã
        confirmBackGround.SetActive(false);
    }
    public void QuitButtonFunc()
    {
        Debug.Log("Quit");
        ResetInvenAndEquip();
        Application.Quit();
    }
    public void QuitConfirmFunc()
    {
        Debug.Log("Quit Confirm");
        quitFlag = true;
        if (Manager.Instance.GetNowScene().name == SceneName.DungeonScene.ToString())
            confirmBackGround.SetActive(true);
        else
            QuitButtonFunc();
    }
    public void QuitConfirmDenyFunc()
    {
        Debug.Log("Quit Confirm Deny");
        quitFlag = false;
        confirmBackGround.SetActive(false);
    }
    public void ResetInvenAndEquip()
    {
        if (Manager.Instance.GetNowScene().name == SceneName.DungeonScene.ToString())
        {
            Manager.Inven.ItemBoxReset(ItemBoxType.Inventory);
            Manager.Inven.ResetEquipSlots();
        }
    }
    public void ConfirmButtonFunc()
    {
        if (quitFlag == true)
            QuitButtonFunc();
        else
        {
            confirmBackGround.SetActive(false);
            confirmBackGround.transform.root.GetComponent<SoundManager>().SettingUIActive();
            ResetInvenAndEquip();
            Manager.Instance.LoadScene(SceneName.MainLobbyScene);
        }
    }
    public void BackToLobby()
    {
        Debug.Log("BackToLobby Confirm");
        if (Manager.Instance.GetNowScene().name == SceneName.DungeonScene.ToString())
            confirmBackGround.SetActive(true);
    }
}
