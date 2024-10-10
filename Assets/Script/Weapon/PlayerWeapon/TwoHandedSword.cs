using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoHandedSword : Weapon
{
    public override void AttackStart(int level)
    {
        base.AttackStart(level);

        switch (level)
        {
            case 0:
                Global.sfx.Play(Global.Sound.twohandedStab, transform.position);
                break;
            case 1:
                Global.sfx.Play(Global.Sound.twohandedSwing, transform.position);
                break;
            case 2:
                Global.sfx.Play(Global.Sound.twohandedSwing, transform.position);
                break;
        }
    }
}
