using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword: Weapon
{
    public override void AttackStart(int level)
    {
        base.AttackStart(level);

        switch(level)
        {
            case 0:
                Global.sfx.Play(Global.Sound.onehandedSwing1, transform.position);
                break;
            case 1:
                Global.sfx.Play(Global.Sound.onehandedSwing2, transform.position);
                break;
            case 2:
                Global.sfx.Play(Global.Sound.onehandedStab, transform.position);
                break;
        }
    }

}
