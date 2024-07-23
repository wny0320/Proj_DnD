using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float pitch = 0f; // ����
    float yaw = 0f; // ����

    //���� ���� �ν����Ϳ��� ����� ���� ���� ���� �ؾߵ�
    Animator animator = null;

    Transform eye = null;
    Transform spine = null;

    void Start()
    {

    }

    void LateUpdate()
    {
        if (Manager.Game.Player == null) return;
        else if(animator == null)
        {
            animator = Manager.Game.Player.GetComponent<Animator>();
            eye = animator.GetBoneTransform(HumanBodyBones.Head);
            spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        }

        CameraMove();
        transform.position = eye.position;
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
        spine.localRotation = Quaternion.Euler(0, 0, pitch);
    }
}
