using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : BaseState
{
    #region Camera
    Camera cam;
    float pitch = 0f; // 수직
    float yaw = 0f; // 수평
    #endregion

    Vector3 dir = Vector3.zero;
    Transform transform;

    public PlayerMoveState(BaseController controller, Rigidbody rb = null, Animator animator = null) : base(controller, rb, animator)
    {
        if(cam == null) cam = Camera.main;

        Manager.Input.PlayerMove -= PlayerMove;
        Manager.Input.CameraMove -= CameraMove;

        Manager.Input.PlayerMove += PlayerMove;
        Manager.Input.CameraMove += CameraMove;

        transform = controller.transform;
    }

    public override void OnFixedUpdate()
    {
    }

    public override void OnStateEnter()
    {
    }

    public override void OnStateExit()
    {
    }

    public override void OnStateUpdate()
    {
    }

    private void CameraMove()
    {
        //플레이어가 인벤 열 때, 죽었을 때 안움직이게 조건 추가애햐됨

        yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * Manager.Input.mouseSpeed;
        pitch -= Input.GetAxis("Mouse Y") * Manager.Input.mouseSpeed;

        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.localRotation = Quaternion.Euler(0, yaw, 0);
        cam.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    private void PlayerMove()
    {
        dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        dir = transform.TransformDirection(dir) * 3; //임시 스피드

        rb.velocity = dir;
    }
}
