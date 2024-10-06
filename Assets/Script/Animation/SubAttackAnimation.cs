using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubAttackAnimation : StateMachineBehaviour
{
    [SerializeField] string triggerName;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(triggerName);
        //Global.PlayerWeapon.AttackEnd();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Manager.Game.isPlayerAttacking = true;
        Global.PlayerWeapon.SubAttackStart();
    }
}
