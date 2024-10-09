using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeController : MonoBehaviour
{
    //≈ª√‚±∏ πÆ
    private Transform door;

    private void Start()
    {
        door = transform.GetChild(0);
        Manager.Game.escapeList.Add(this);
        Manager.Game.escapeCount++;
    }

    public void EscapeDoorOpen()
    {
        door.DOLocalMove(new Vector3(0, -2 ,0), 3f).SetEase(Ease.InQuad);
        GetComponent<Collider>().enabled = true;
        //Debug.Log($"{name} open");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Manager.Game.OnGameEnd(true, SceneName.MainLobbyScene);
        }
    }
}
