using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrusaderAttack3State : BaseState
{
    EnemyWeapon weapon;

    //�� ���� ��, �ٽ� movestate�� ����
    public CrusaderAttack3State(BaseController controller, Rigidbody rb = null, Animator animator = null, EnemyWeapon weapon = null) : base(controller, rb, animator)
    {
        this.weapon = weapon;
    }

    public override void OnFixedUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95)
            {
                weapon.AttackEnd();
                controller.ChangeState(EnemyState.Move);
            }
        }
    }

    public override void OnStateEnter()
    {
        weapon.AttackStart();
    }

    public override void OnStateExit()
    {
    }

    public override void OnStateUpdate()
    {

    }

}
