using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager
{
    // ���� �Ŵ��� - ���� �帧 ���� �ۼ�
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
        if (!isPlayerAlive || Player == null) return;

        //�ð�ui
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

        //player hp ui
        HpUI.value = Player.GetComponent<BaseController>().stat.Hp;
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
