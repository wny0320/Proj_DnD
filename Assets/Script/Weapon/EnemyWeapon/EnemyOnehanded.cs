using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOnehanded : EnemyWeapon
{
    public override void AttackStart()
    {
        if (controller == null) controller = transform.root.GetComponent<EnemyController>();

        totalDamage = controller.stat.Attack;

        hittedObject.Clear();

        col.enabled = true;
    }

    public override void AttackEnd()
    {
        col.enabled = false;
    }
}
