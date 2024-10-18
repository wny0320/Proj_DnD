using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : Weapon
{
    [SerializeField] GameObject LeftHand;

    public override void AttackStart(int level)
    {
        totalDamage = Manager.Game.Player.GetComponent<PlayerController>().stat.Attack;
        totalDamage += level;

        Global.sfx.Play(Global.Sound.punching);

        if (level == 0)
        {
            //������
            cols[0].enabled = true;
            //�޼�
            LeftHand.GetComponent<Collider>().enabled = false;
        }
        else if(level == 1)
        {
            //�޼�
            LeftHand.GetComponent<Collider>().enabled = true;
            //������
            cols[0].enabled = false;
        }

        hittedObject.Clear();
    }

    public override void AttackEnd()
    {
        cols[0].enabled = false;
        LeftHand.GetComponent<Collider>().enabled = false;
    }

    public override void SetWeaponInfo()
    {
        damage = 0;
        SetGlobalWeapon();
    }

    private void OnDisable()
    {
        LeftHand.SetActive(false);
    }
    private void OnEnable()
    {
        SetGlobalWeapon();
        LeftHand.SetActive(true);
    }
}
