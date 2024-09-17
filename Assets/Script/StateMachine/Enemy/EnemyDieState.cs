using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDieState : BaseState
{
    public EnemyDieState(BaseController controller, Rigidbody rb = null, Animator animator = null) : base(controller, rb, animator)
    {
    }

    public override void OnFixedUpdate()
    {
    }

    public override void OnStateEnter()
    {
        animator.Play("Die");
        controller.isAlive = false;
    }

    public override void OnStateExit()
    {

    }

    public override void OnStateUpdate()
    {
    }
}
