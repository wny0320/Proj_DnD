using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartButton : MonoBehaviour
{
    public void GameStart()
    {
        Manager.Instance.LoadScene(SceneName.DungeonScene.ToString());
    }
    public void TestLobbyMove()
    {
        Manager.Instance.LoadScene(SceneName.MainLobbyScene.ToString());
    }
}
