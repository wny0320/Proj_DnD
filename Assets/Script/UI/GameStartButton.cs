using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartButton : MonoBehaviour
{
    public void GameStart()
    {
        Manager.Instance.LoadScene(SceneName.DungeonScene);
    }

    public void TestLobbyMove()
    {
        Debug.Log(SceneName.MainLobbyScene.ToString());
        Manager.Instance.LoadScene(SceneName.MainLobbyScene);
    }
}
