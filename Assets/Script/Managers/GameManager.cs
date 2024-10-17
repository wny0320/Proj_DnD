using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager
{
    // 게임 매니저 - 게임 흐름 관련 작성
    public GameObject Player;

    public bool isPlayerAlive = true;
    public bool isCursorLock = true;
    public bool isPlayerAttacking = false;
    public bool isSettingUIActive = false;

    public GameObject GameUI;
    private Slider TimeUI;
    private Slider HpUI;

    private float gameTimer = 600f;
    private float passedTimer = 0f;

    //탈출구 관련
    public List<EscapeController> escapeList = new();
    public int escapeCount = 0;

    public void OnGameSceneLoad()
    {
        isPlayerAlive = true;
        isSettingUIActive = false;

        CursorLock(true);
        escapeList.Clear();

        TimeUI = GameUI.transform.GetChild(0).GetComponent<Slider>();
        HpUI = GameUI.transform.GetChild(1).GetComponent<Slider>();

        passedTimer = 0f;

        ResetEscape();
    }

    public void OnFixedUpdate()
    {
        if (Manager.Instance.sceneName != SceneName.DungeonScene || Player == null) return;

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
            return;
        }

        CheckTimeToEscape();
    }

    public async void OnGameEnd(bool isAlive, SceneName sceneName)
    {
        Manager.Instance.sceneName = sceneName;

        if (!isAlive)
        {
            HpUI.value = 0f;

            //사망시 인벤 삭제
            Manager.Inven.ItemBoxReset(ItemBoxType.Inventory);
            Manager.Inven.ResetEquipSlots();
        }
        Manager.Inven.ConcealInvenCanvasByBt();

        await Task.Delay(3000);

        //씬 변경
        Manager.Instance.LoadScene(SceneName.MainLobbyScene);
    }

    public void CursorLock(bool cursurLock)
    {
        isCursorLock = cursurLock;

        if(cursurLock)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }

    private void ResetEscape()
    {
        escapeList.Clear();
    }

    private void CheckTimeToEscape()
    {
        if(TimeUI.value > (10f - escapeCount)/10)
        {
            int n = Random.Range(0, escapeCount);
            escapeList[n].EscapeDoorOpen();
            escapeList.RemoveAt(n);
            escapeCount--;
        }
    }
}
