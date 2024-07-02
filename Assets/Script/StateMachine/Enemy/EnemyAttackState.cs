using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        //Debug.Log("AATTAACCKK");
    }

    public override void OnStateExit()
    {
    }

    public override void OnStateUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95)
                controller.ChangeState(EnemyState.Move);
        }
    }

}
