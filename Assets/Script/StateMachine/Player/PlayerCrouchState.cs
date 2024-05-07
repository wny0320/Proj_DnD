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

        // �ִϸ��̼� bool or trigger
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
        //��ũ����� ���߿� �ִϸ��̼� ���� �ؾߵɵ�
        //���� transform.localScale �κ� �����ϸ� �ɵ���
    }
}
