using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAttack : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Manager.Game.isPlayerAttacking = false;
        Global.PlayerWeapon.AttackEnd();
    }
}
