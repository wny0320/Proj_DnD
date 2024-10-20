using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected int damage = 0;
    protected float totalDamage;

    protected Collider[] cols;
    protected List<IReceiveAttack> hittedObject = new();


    private void Start()
    {
        gameObject.layer = 0;

        cols = GetComponents<Collider>();
        foreach(Collider collider in cols)
            collider.enabled = false;

        SetWeaponInfo();
    }


    public void SetGlobalWeapon()
    {
        Global.PlayerWeapon = this;
    }

    public virtual void SetWeaponInfo()
    {
        Item itemInfo = GetComponent<Item3D>().myItem;
        damage = itemInfo.itemStat.Attack;

        SetGlobalWeapon();
    }

    public virtual void AttackStart(int level)
    {
        //Debug.Log("attack start");

        totalDamage = damage;
        totalDamage += (totalDamage * (0.1f * level));

        hittedObject.Clear();

        foreach (Collider collider in cols)
            collider.enabled = true;
    }

    public virtual void AttackEnd()
    {
        //애니메이션 진입 시 무조건 한 번 먼저 실행됨
        //Debug.Log("Attack End");

        foreach (Collider collider in cols)
            collider.enabled = false;
    }

    public virtual void SubAttackStart()
    {
        totalDamage = damage;

        hittedObject.Clear();

        foreach (Collider collider in cols)
            collider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        IReceiveAttack attacked = GetHittedObj(other.transform);
        if (attacked == null) return;
        if (hittedObject.Contains(attacked)) return;
        hittedObject.Add(attacked);

        attacked.OnHit(totalDamage);
    }   

    IReceiveAttack GetHittedObj(Transform trans)
    {
        if (trans.GetComponent<IReceiveAttack>() == null)
        {
            if (trans.parent == null) return null;
            return GetHittedObj(trans.parent);
        }
        else
            return trans.GetComponent<IReceiveAttack>();
    }
}
