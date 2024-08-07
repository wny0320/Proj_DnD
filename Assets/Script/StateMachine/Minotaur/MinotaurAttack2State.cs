using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MinotaurAttack2State : BaseState
{
    EnemyWeapon weapon;
    bool isComboAttack = true;

    List<IReceiveAttack> attackList = new();

    //적 공격 후, 다시 movestate로 변경
    public MinotaurAttack2State(BaseController controller, Rigidbody rb = null, Animator animator = null, EnemyWeapon weapon = null) : base(controller, rb, animator)
    {
        this.weapon = weapon;
    }

    public override void OnFixedUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.15 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 33)
            {
                KickAttack();
            }

            if (isComboAttack && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.79)
            {
                animator.Play("Attack3");
                controller.ChangeState(EnemyState.Attack3);
                return;
            }

            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.94)
            {
                controller.ChangeState(EnemyState.Move);
            }
        }
    }

    public override void OnStateEnter()
    {
        attackList.Clear();

        int n = Random.Range(0, 10);
        isComboAttack = n > 4 ? true : false;
    }

    public override void OnStateExit()
    {
        attackList.Clear();
    }

    public override void OnStateUpdate()
    {

    }

    private void KickAttack()
    {
        Collider[] cols = Physics.OverlapBox(controller.transform.position + controller.transform.forward * 1.5f + Vector3.up * 1.5f,
            new Vector3(0.5f, 0.75f, 1f));

        foreach(Collider col in cols)
        {
            IReceiveAttack attacked = col.GetComponent<IReceiveAttack>();
            if (attacked == null) continue;
            if(attackList.Contains(attacked)) continue;
            if(attacked == controller.GetComponent<IReceiveAttack>()) continue;

            attacked.OnHit(controller.stat.Attack * 1.5f);

            attackList.Add(attacked);
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawCube(transform.position + transform.forward * 1.5f + Vector3.up * 1.5f, new Vector3(1, 1.5f, 2));
    //}
}
