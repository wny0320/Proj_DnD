using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeController : MonoBehaviour
{
    //탈출구 문
    private Transform door;

    private void Start()
    {
        door = transform.GetChild(0);
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
            //이쪽에 탈출 관련 함수
        }
    }
}
