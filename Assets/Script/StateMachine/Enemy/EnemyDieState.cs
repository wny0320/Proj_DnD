using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDieState : BaseState
{
    //적 추적 및 이동
    private bool isFind = false;

    public EnemyDieState(BaseController controller, Rigidbody rb = null, Animator animator = null) : base(controller, rb, animator)
    {
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
}
