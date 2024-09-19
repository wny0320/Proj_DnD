using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager
{
    // 게임 매니저 - 게임 흐름 관련 작성
    public GameObject Player;

    public bool isPlayerAlive = true;
    public bool isCursorLock = true;

    private GameObject GameUI;
    private Slider TimeUI;
    private Slider HpUI;

    private float gameTimer = 300f;
    private float passedTimer = 0f;

    public void OnAwake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = 60;
        GameUI = GameObject.Find("GameUI");
        TimeUI = GameUI.transform.GetChild(0).GetComponent<Slider>();
        HpUI = GameUI.transform.GetChild(1).GetComponent<Slider>();

        passedTimer = 0f;
    }

    public void OnFixedUpdate()
    {
        //player hp ui

        if (Player == null) return;

        HpUI.value = (float)Player.GetComponent<BaseController>().stat.Hp / (float)Player.GetComponent<BaseController>().stat.MaxHp;
        if (!isPlayerAlive)
            return;

        //시간ui
        passedTimer += Time.fixedDeltaTime;
        TimeUI.value = passedTimer / gameTimer;
        if(passedTimer >= gameTimer+5)
        {
            isPlayerAlive = false;
            Player.GetComponent<PlayerController>().ChangeState(PlayerState.Die);
            HpUI.value = 0;
            OnGameEnd();
            return;
        }
    }

    public void OnGameEnd()
    {

    }

    public void CursorLock(bool cursurLock)
    {
        isCursorLock = cursurLock;

        if(cursurLock)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }
}
