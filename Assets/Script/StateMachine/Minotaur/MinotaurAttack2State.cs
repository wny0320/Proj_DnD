using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MinotaurAttack2State : BaseState
{
    EnemyWeapon weapon;
    bool isComboAttack = true;

    //적 공격 후, 다시 movestate로 변경
    public MinotaurAttack2State(BaseController controller, Rigidbody rb = null, Animator animator = null, EnemyWeapon weapon = null) : base(controller, rb, animator)
    {
        this.weapon = weapon;
    }

    public override void OnFixedUpdate()
    {
    }

    public override void OnStateEnter()
    {
        //Debug.Log("AATTAACCKK");
        int n = Random.Range(0, 10);
        //isComboAttack = n > 4 ? true : false;

        weapon.AttackStart();
    }

    public override void OnStateExit()
    {
    }

    public override void OnStateUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95)
            {
                weapon.AttackEnd();
                controller.ChangeState(EnemyState.Move);
            }

            if (isComboAttack)
            {
                animator.SetTrigger("EnemyAttack");
                controller.ChangeState(EnemyState.Attack3);
            }
        }
    }

}
