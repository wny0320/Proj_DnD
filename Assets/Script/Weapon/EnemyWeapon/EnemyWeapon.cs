using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    protected float totalDamage;

    protected BaseController controller;
    protected Collider col;
    protected List<IReceiveAttack> hittedObject = new();

    private void Start()
    {
        col = GetComponent<Collider>();
    }

    public virtual void AttackStart(){ }

    public virtual void AttackEnd(){ }

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
        {
            if (trans.GetComponent<IReceiveAttack>() == transform.root.GetComponent<IReceiveAttack>())
                return null;
            return trans.GetComponent<IReceiveAttack>();
        }
    }
}
