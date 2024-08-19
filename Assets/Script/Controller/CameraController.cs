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

    //������ �Ⱦ�
    [SerializeField] private float reach = 1.5f;
    [SerializeField] private LayerMask layerMask;
    private RaycastHit hit;
    private bool isPickupActivate = false;

    private void FixedUpdate()
    {
        CheckInteractiveObj();
        Interactive(hit);
    }

    void LateUpdate()
    {
        SetCam();
    }

    private void SetCam()
    {
        if (Manager.Game.Player == null) return;
        else if (animator == null)
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

    private void Interactive(RaycastHit hitted)
    {
        if(isPickupActivate && Input.GetKeyDown(KeyCode.F))
        {
            if (hit.transform.tag.Equals("Item"))
            {
                ItemFunc();
            }
            else if (hit.transform.tag.Equals("Door"))
            {
                DoorFunc();
            }
        }
    }

    private void CheckInteractiveObj()
    {
        //Debug.DrawRay(transform.position, transform.forward * reach, Color.red);

        if(Physics.Raycast(transform.position, transform.forward, out hit, reach, layerMask))
            isPickupActivate = true;
        else
            isPickupActivate = false;
    }

    private void ItemFunc()
    {
        Debug.Log("item");
    }

    private void DoorFunc()
    {
        Debug.Log("Door");

    }
}
