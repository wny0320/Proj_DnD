using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float pitch = 0f; // ����
    float yaw = 0f; // ����

    //���� ���� �ν����Ϳ��� ����� ���� ���� ���� �ؾߵ�
    [SerializeField]
    Transform eye = null;

    void Start()
    {

    }

    void LateUpdate()
    {
        if (Manager.Game.Player == null) return;

        CameraMove();
        transform.position = eye.position;
        //transform.position += transform.forward * 0.2f;
    }

    private void CameraMove()
    {
        if (!Manager.Game.isPlayerAlive) return;

        //�÷��̾ �κ� �� ��, �׾��� �� �ȿ����̰� ���� �߰������

        yaw += Input.GetAxis("Mouse X") * Manager.Input.mouseSpeed;
        pitch -= Input.GetAxis("Mouse Y") * Manager.Input.mouseSpeed;

        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.localRotation = Quaternion.Euler(pitch, yaw, 0);
        eye.root.localRotation = Quaternion.Euler(0, yaw, 0);
    }
}
