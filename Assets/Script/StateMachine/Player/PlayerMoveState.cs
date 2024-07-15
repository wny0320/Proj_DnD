using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : BaseState
{
    const string PLAYER_MOVE = "PlayerMove";
    const string PLAYER_JUMP = "PlayerJump";
    const string PLAYER_ATTACK = "PlayerAttack";

    Vector3 dir = Vector3.zero;
    Transform transform;

    bool isGrounded = true;

    public PlayerMoveState(BaseController controller, Rigidbody rb = null, Animator animator = null) : base(controller, rb, animator)
    {
        Manager.Input.PlayerMove -= PlayerMove;
        Manager.Input.PlayerAttack -= PlayerAttack;

        Manager.Input.PlayerMove += PlayerMove;
        Manager.Input.PlayerAttack += PlayerAttack;

        transform = controller.transform;
    }

    public override void OnFixedUpdate()
    {
        //Debug.Log("isground" + isGrounded);
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

        CheckGround();

        if (Input.GetKeyDown(KeyCode.LeftControl))
            controller.ChangeState(PlayerState.Crouch);
    }

    private void PlayerMove()
    {
        if (!controller.isAlive) return;

        dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        dir = transform.TransformDirection(dir) * controller.stat.MoveSpeed;

        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = (dir - velocity);
        velocityChange.y = 0;
        
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
        if (Mathf.Abs(velocity.x) > 0.2f || Mathf.Abs(velocity.z) > 0.2f) animator.SetBool(PLAYER_MOVE, true);
        else animator.SetBool(PLAYER_MOVE, false);
        if (isGrounded && Input.GetKeyDown(KeyCode.Space)) Jump();
    }

    private void Jump()
    {
        animator.SetTrigger(PLAYER_JUMP);
        rb.AddForce(Vector3.up * controller.stat.JumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    private void CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + (transform.localScale.y * .5f), transform.position.z);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = 1.5f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        //Debug.DrawRay(origin, direction, Color.red);
    }

    private void PlayerAttack()
    {
        if (!controller.isAlive) return;

        if (Input.GetMouseButtonDown(0))
            animator.SetTrigger(PLAYER_ATTACK);
    }
}
