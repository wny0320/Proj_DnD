using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItemAdd : MonoBehaviour
{
    public void AddButton()
    {
        string targetItemName = "exampleItem";
        Item targetItem = Manager.Data.itemData[targetItemName];
        //Manager.Inven.AddItem(targetItem);
    }
    public void AddButton2()
    {
        string targetItemName = "BattleAxe";
        Item targetItem = Manager.Data.itemData[targetItemName];
        //Manager.Inven.AddItem(targetItem);
    }
    public void SceneMoveTestLobby()
    {
        Manager.Instance.LoadScene("LobbyMerchantWork");
        Manager.Inven.ConcealInvenCanvasByBt();
        Cursor.lockState = CursorLockMode.None;
    }
    public void SceneMoveTestDungeon()
    {
        Manager.Instance.LoadScene("PlayerAndInven");
        Cursor.lockState = CursorLockMode.Locked;
    }
}
