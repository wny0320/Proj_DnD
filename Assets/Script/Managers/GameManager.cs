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
    public bool isPlayerAttacking = false;

    private GameObject GameUI;
    private Slider TimeUI;
    private Slider HpUI;

    private float gameTimer = 300f;
    private float passedTimer = 0f;

    //Ż�ⱸ ����
    private List<EscapeController> escapeList = new();
    private int escapeCount = 0;

    public void OnAwake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = 60;
        GameUI = GameObject.Find("GameUI");
        TimeUI = GameUI.transform.GetChild(0).GetComponent<Slider>();
        HpUI = GameUI.transform.GetChild(1).GetComponent<Slider>();

        passedTimer = 0f;

        GetEscape();
    }

    public void OnFixedUpdate()
    {
        //player hp ui

        if (Player == null) return;

        HpUI.value = (float)Player.GetComponent<BaseController>().stat.Hp / (float)Player.GetComponent<BaseController>().stat.MaxHp;
        if (!isPlayerAlive)
            return;

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

        CheckTimeToEscape();
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

    private void GetEscape()
    {
        Transform EscapeWay = GameObject.Find("EscapeWay").transform;
        foreach(Transform child in EscapeWay)
            escapeList.Add(child.GetComponent<EscapeController>());
        escapeCount = escapeList.Count;
    }

    private void CheckTimeToEscape()
    {
        if(TimeUI.value > (10f - escapeCount)/10)
        {
            int n = Random.Range(0, escapeCount + 1);
            escapeList[n].EscapeDoorOpen();
            escapeList.RemoveAt(n);
            escapeCount--;
        }
    }
}
