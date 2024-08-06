using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrusaderAttack2State : BaseState
{
    EnemyWeapon weapon;
    bool isComboAttack = true;

    //적 공격 후, 다시 movestate로 변경
    public CrusaderAttack2State(BaseController controller, Rigidbody rb = null, Animator animator = null, EnemyWeapon weapon = null) : base(controller, rb, animator)
    {
        this.weapon = weapon;
    }

    public override void OnFixedUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            if (isComboAttack && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.79)
            {
                animator.Play("Attack3");
                controller.ChangeState(EnemyState.Attack2);
                return;
            }

            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.94)
            {
                weapon.AttackEnd();
                controller.ChangeState(EnemyState.Move);
            }
        }
    }

    public override void OnStateEnter()
    {
        int n = Random.Range(0, 10);
        isComboAttack = n > 4 ? true : false;

        weapon.AttackStart();
    }

    public override void OnStateExit()
    {

    }

    public override void OnStateUpdate()
    {

    }

}
