using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : BaseState
{
    #region Camera
    Camera cam;
    float pitch = 0f; // ����
    float yaw = 0f; // ����
    #endregion

    Vector3 dir = Vector3.zero;
    Transform transform;

    bool isCrouch = false;
    bool isGrounded = true;
    float speedReduction = 2f;

    float walkSpeed = 5f; //�ӽ� ���ǵ�
    float jumpforce = 5f; //�ӽ� ����
    Vector3 originalScale; //�ӽ� ũ��

    public PlayerMoveState(BaseController controller, Rigidbody rb = null, Animator animator = null) : base(controller, rb, animator)
    {
        if(cam == null) cam = Camera.main;

        Manager.Input.PlayerMove -= PlayerMove;
        Manager.Input.CameraMove -= CameraMove;

        Manager.Input.PlayerMove += PlayerMove;
        Manager.Input.CameraMove += CameraMove;

        transform = controller.transform;
        originalScale = transform.localScale;
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
        CheckGround();
    }

    private void CameraMove()
    {
        //�÷��̾ �κ� �� ��, �׾��� �� �ȿ����̰� ���� �߰������

        yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * Manager.Input.mouseSpeed;
        pitch -= Input.GetAxis("Mouse Y") * Manager.Input.mouseSpeed;

        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.localRotation = Quaternion.Euler(0, yaw, 0);
        cam.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    private void PlayerMove()
    {
        dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        dir = transform.TransformDirection(dir) * walkSpeed;

        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = (dir - velocity);
        velocityChange.y = 0;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space)) Jump();
        Crouch();
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpforce, ForceMode.Impulse);
        isGrounded = false;
    }

    private void Crouch()
    {
        //��ũ����� ���߿� �ִϸ��̼� ���� �ؾߵɵ�
        //���� transform.localScale �κ� �����ϸ� �ɵ���

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (isCrouch) return;

            transform.localScale = new Vector3(originalScale.x, originalScale.y / 2, originalScale.z); 
            walkSpeed /= speedReduction;
            isCrouch = true;
        }
        else
        {
            if (!isCrouch) return;

            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            walkSpeed *= speedReduction;
            isCrouch = false;
        }
    }

    private void CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = .75f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
