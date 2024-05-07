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
    }

    public override void OnStateEnter()
    {
        controller.stat.MoveSpeed /= speedReduction;

        // 애니메이션 bool or trigger
    }

    public override void OnStateExit()
    {
        controller.stat.MoveSpeed *= speedReduction;
    }

    public override void OnStateUpdate()
    {
        Crouch();

        if (Input.GetKeyUp(KeyCode.LeftControl))
            controller.ChangeState(PlayerState.Move);
    }

    private void Crouch()
    {
        //웅크리기는 나중에 애니메이션 보고 해야될듯
        //밑의 transform.localScale 부분 변경하면 될듯함
    }
}
