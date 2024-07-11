using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float pitch = 0f; // 수직
    float yaw = 0f; // 수평

    //현재 눈이 인스펙터에서 끌어다 쓰고 있음 수정 해야됨
    [SerializeField]
    Transform eye = null;

    void Start()
    {

    }

    void LateUpdate()
    {
        CameraMove();
        transform.position = eye.position;
        transform.position += transform.forward * 0.4f;
    }

    private void CameraMove()
    {
        if (!Manager.Game.isPlayerAlive) return;

        //플레이어가 인벤 열 때, 죽었을 때 안움직이게 조건 추가애햐됨

        yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * Manager.Input.mouseSpeed;
        pitch -= Input.GetAxis("Mouse Y") * Manager.Input.mouseSpeed;

        pitch = Mathf.Clamp(pitch, -90f, 90f);

        eye.root.localRotation = Quaternion.Euler(0, yaw, 0);
        transform.localRotation = Quaternion.Euler(pitch, yaw, 0);
    }
}
