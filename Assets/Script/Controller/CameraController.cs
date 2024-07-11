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
        CameraMove();
        transform.position = eye.position;
        transform.position += transform.forward * 0.4f;
    }

    private void CameraMove()
    {
        if (!Manager.Game.isPlayerAlive) return;

        //�÷��̾ �κ� �� ��, �׾��� �� �ȿ����̰� ���� �߰������

        yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * Manager.Input.mouseSpeed;
        pitch -= Input.GetAxis("Mouse Y") * Manager.Input.mouseSpeed;

        pitch = Mathf.Clamp(pitch, -90f, 90f);

        eye.root.localRotation = Quaternion.Euler(0, yaw, 0);
        transform.localRotation = Quaternion.Euler(pitch, yaw, 0);
    }
}
