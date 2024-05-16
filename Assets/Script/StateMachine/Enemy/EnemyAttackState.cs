using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : BaseState
{
    //적 공격 후, 다시 movestate로 변경
    public EnemyAttackState(BaseController controller, Rigidbody rb = null, Animator animator = null) : base(controller, rb, animator)
    {
    }

    public override void OnFixedUpdate()
    {
    }

    public override void OnStateEnter()
    {
        Debug.Log("AATTAACCKK");
    }

    public override void OnStateExit()
    {

    }

    public override void OnStateUpdate()
    {

    }

}
