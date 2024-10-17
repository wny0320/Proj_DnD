using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : BaseState
{
    const string PLAYER_MOVE = "PlayerMove";
    const string PLAYER_JUMP = "PlayerJump";
    const string PLAYER_ATTACK = "PlayerAttack";
    const string PLAYER_SUBATTACK = "PlayerSubAttack";

    Vector3 dir = Vector3.zero;
    Transform transform;

    float cantJumpTime = 0.2f;
    bool canJump = true;

    public PlayerMoveState(BaseController controller, Rigidbody rb = null, Animator animator = null) : base(controller, rb, animator)
    {
        Manager.Input.PlayerMove = PlayerMove;
        Manager.Input.PlayerAttack = PlayerAttack;

        transform = controller.transform;
    }

    public override void OnFixedUpdate()
    {
        //Debug.Log("isground" + isGrounded);
        CheckGround();

        if (!canJump) cantJumpTime -= Time.fixedDeltaTime;
        if (cantJumpTime <= 0f) { cantJumpTime = 0.2f; canJump = true; }
    }

    public override void OnStateEnter()
    {
    }

    public override void OnStateExit()
    {
    }

    public override void OnStateUpdate()
    {
        if (!controller.isAlive) return;

        if (Input.GetKeyDown(KeyCode.LeftControl))
            controller.ChangeState(PlayerState.Crouch);
    }

    private void PlayerMove()
    {
        if (!controller.isAlive) return;

        dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        dir = transform.TransformDirection(dir) * controller.stat.MoveSpeed;
        
        Vector3 velocity = rb.velocity;
        //Vector3 velocityChange = (dir - velocity);
        //velocityChange.y = 0;

        rb.velocity = new Vector3(dir.x, rb.velocity.y, dir.z);
        //rb.AddForce(velocityChange, ForceMode.VelocityChange);
        if (Mathf.Abs(velocity.x) > 0.2f || Mathf.Abs(velocity.z) > 0.2f) animator.SetBool(PLAYER_MOVE, true);
        else animator.SetBool(PLAYER_MOVE, false);
        if (CheckGround() && canJump && Input.GetKeyDown(KeyCode.Space)) Jump();
    }

    private void Jump()
    {
        canJump = false;
        animator.SetTrigger(PLAYER_JUMP);
        rb.AddForce(Vector3.up * controller.stat.JumpForce, ForceMode.Impulse);
    }

    private bool CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + (transform.localScale.y * .5f), transform.position.z);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = 1f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            return true;
        }
        else
        {
            return false;
        }

    }

    private void PlayerAttack()
    {
        if (!controller.isAlive || Manager.Inven.canvasVisualFlag || Manager.Game.isSettingUIActive) return;

        if (Input.GetMouseButtonDown(0))
            animator.SetTrigger(PLAYER_ATTACK);
        else if (Input.GetMouseButtonDown(1))
            animator.SetTrigger(PLAYER_SUBATTACK);
    }
}
