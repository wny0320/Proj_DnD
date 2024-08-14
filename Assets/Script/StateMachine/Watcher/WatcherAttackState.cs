using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WatcherAttackState : BaseState
{
    EnemyWeapon weapon;
    bool isAttacking = false;

    //적 공격 후, 다시 movestate로 변경
    public WatcherAttackState(BaseController controller, Rigidbody rb = null, Animator animator = null, EnemyWeapon weapon = null) : base(controller, rb, animator)
    {
        this.weapon = weapon;
    }

    public override void OnFixedUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("BattleIdle"))
        {
            if(Random.Range(0, 10) < 5)
                animator.Play("Attack1");
            else
                animator.Play("Attack2");
        }
        
    }

    public override void OnStateEnter()
    {
    }

    public override void OnStateExit()
    {
    }

    public override void OnStateUpdate()
    {
        OnAttack();
    }

    private void OnAttack()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            if (!isAttacking && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4)
            {
                weapon.AttackStart();
                isAttacking = true;
            }
            else if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.65)
            {
                weapon.AttackEnd();
            }

            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.89)
            {
                isAttacking = false;
                controller.ChangeState(EnemyState.Move);
            }
        }
        else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            if (!isAttacking && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.34)
            {
                weapon.AttackStart();
                isAttacking = true;
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.44)
            {
                weapon.AttackEnd();
            }

            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.89)
            {
                isAttacking = false;
                controller.ChangeState(EnemyState.Move);
            }
        }
    }
}
