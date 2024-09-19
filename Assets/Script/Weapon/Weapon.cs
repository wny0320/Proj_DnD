using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected int damage;
    protected float totalDamage;
    protected float speedDebuff;

    protected Collider col;
    protected List<IReceiveAttack> hittedObject = new();

    
    public void SetGlobalWeapon()
    {
        Global.PlayerWeapon = this;
    }

    public virtual void SetWeaponInfo() { }

    public virtual void AttackStart(int level)
    {
        //Debug.Log("attack start");
        totalDamage = damage + Manager.Game.Player.GetComponent<PlayerController>().stat.Attack;
        totalDamage += (totalDamage * (0.1f * level));

        hittedObject.Clear();

        col.enabled = true;
    }

    public void AttackEnd()
    {
        //Debug.Log("Attack End");

        col.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        IReceiveAttack attacked = other.transform.root.GetComponent<IReceiveAttack>();
        if (attacked == null) return;
        if (hittedObject.Contains(attacked)) return;
        hittedObject.Add(attacked);

        attacked.OnHit(totalDamage);
    }
}
