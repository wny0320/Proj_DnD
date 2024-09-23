using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimation : StateMachineBehaviour
{
    [SerializeField] string triggerName;
    [SerializeField] int level;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(triggerName);
        //Global.PlayerWeapon.AttackEnd();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Manager.Game.isPlayerAttacking = true;
        Global.PlayerWeapon.AttackStart(level);
    }
}
