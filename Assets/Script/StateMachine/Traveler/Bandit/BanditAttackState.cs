using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BanditAttackState : BaseState
{
    EnemyWeapon weapon;
    int atk = 0;

    //적 공격 후, 다시 movestate로 변경
    public BanditAttackState(BaseController controller, Rigidbody rb = null, Animator animator = null, EnemyWeapon weapon = null) : base(controller, rb, animator)
    {
        this.weapon = weapon;
    }

    public override void OnFixedUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.84)
        {
            weapon.AttackEnd();
            controller.ChangeState(EnemyState.Move);
            return;
        }
    }

    public override void OnStateEnter()
    {
        atk = Random.Range(1, 4);
        animator.Play($"Attack{atk}");
        weapon.AttackStart();
    }

    public override void OnStateExit()
    {
    }

    public override void OnStateUpdate()
    {

    }

}
