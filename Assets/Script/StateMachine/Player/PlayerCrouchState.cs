using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchState : BaseState
{
    const string PLAYER_CROUCH = "PlayerCrouch";
    private float speedReduction = 2f;

    public PlayerCrouchState(BaseController controller, Rigidbody rb = null, Animator animator = null) : base(controller, rb, animator)
    {

    }

    public override void OnFixedUpdate()
    {
        Crouch();
    }

    public override void OnStateEnter()
    {
        controller.stat.MoveSpeed -= speedReduction;
        animator.SetBool(PLAYER_CROUCH, true);

        controller.GetComponent<CapsuleCollider>().height = 1.3f;
    }

    public override void OnStateExit()
    {
        controller.stat.MoveSpeed += speedReduction;
        animator.SetBool(PLAYER_CROUCH, false);

        controller.GetComponent<CapsuleCollider>().height = 1.75f;
    }

    public override void OnStateUpdate()
    {


    }

    private void Crouch()
    {
        if (Input.GetKeyUp(KeyCode.LeftControl))
            controller.ChangeState(PlayerState.Move);
    }
}
