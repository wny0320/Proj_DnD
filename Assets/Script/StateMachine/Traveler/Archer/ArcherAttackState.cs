using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArcherAttackState : BaseState
{
    EnemyRanged weapon;
    bool isAttacked = false;

    //적 공격 후, 다시 movestate로 변경
    public ArcherAttackState(BaseController controller, Rigidbody rb = null, Animator animator = null, EnemyWeapon weapon = null) : base(controller, rb, animator)
    {
        this.weapon = weapon as EnemyRanged;
    }

    public override void OnFixedUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Recoil"))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.79)
            {
                controller.ChangeState(EnemyState.Move);
                return;
            }
            if (!isAttacked && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5)
            {
                weapon.AttackStart(controller.transform);
                isAttacked = true;
            }
        }
    }

    public override void OnStateEnter()
    {
        isAttacked = false;
    }

    public override void OnStateExit()
    {

    }

    public override void OnStateUpdate()
    {

    }

}
