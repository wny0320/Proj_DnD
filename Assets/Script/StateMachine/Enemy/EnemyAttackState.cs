using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttackState : BaseState
{
    EnemyWeapon weapon;

    //�� ���� ��, �ٽ� movestate�� ����
    public EnemyAttackState(BaseController controller, Rigidbody rb = null, Animator animator = null, EnemyWeapon weapon = null) : base(controller, rb, animator)
    {
        this.weapon = weapon;
    }

    public override void OnFixedUpdate()
    {
    }

    public override void OnStateEnter()
    {
        //Debug.Log("AATTAACCKK");
        weapon.AttackStart();
    }

    public override void OnStateExit()
    {
    }

    public override void OnStateUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95)
            {
                weapon.AttackEnd();
                controller.ChangeState(EnemyState.Move);
            }
        }
    }

}
