using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected int damage;
    protected float totalDamage;

    protected Collider[] cols;
    protected List<IReceiveAttack> hittedObject = new();


    private void Start()
    {
        cols = GetComponents<Collider>();
        SetGlobalWeapon();
        cols.Where(x => x.enabled = false);

        SetWeaponInfo();
    }


    public void SetGlobalWeapon()
    {
        Global.PlayerWeapon = this;
    }

    public void SetWeaponInfo()
    {
        Item itemInfo = GetComponent<Item3D>().myItem;
        damage = itemInfo.itemStat.Attack;

        SetGlobalWeapon();
    }

    public virtual void AttackStart(int level)
    {
        //Debug.Log("attack start");
        totalDamage = damage + Manager.Game.Player.GetComponent<PlayerController>().stat.Attack;
        totalDamage += (totalDamage * (0.1f * level));

        hittedObject.Clear();

        cols.Where(x => x.enabled = true);
    }

    public void AttackEnd()
    {
        //Debug.Log("Attack End");

        cols.Where(x => x.enabled = false);
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
