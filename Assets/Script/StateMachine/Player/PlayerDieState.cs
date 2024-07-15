using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDieState : BaseState
{
    public PlayerDieState(BaseController controller, Rigidbody rb = null, Animator animator = null) : base(controller, rb, animator)
    {

    }

    public override void OnFixedUpdate()
    {
    }

    public override void OnStateEnter()
    {
        animator.SetTrigger("PlayerDead");
        controller.isAlive = false;
        Manager.Game.isPlayerAlive = false;
        Debug.Log("DEAD");
    }

    public override void OnStateExit()
    {
    }

    public override void OnStateUpdate()
    {
    }
}
