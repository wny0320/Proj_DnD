using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    protected float totalDamage;

    protected EnemyController controller;
    protected Collider col;
    protected List<Collider> hittedObject = new();

    private void Start()
    {
        col = GetComponent<Collider>();
    }

    public virtual void AttackStart(){ }

    public virtual void AttackEnd(){ }

    private void OnTriggerEnter(Collider other)
    {
        IReceiveAttack attacked = other.GetComponent<IReceiveAttack>();
        if (attacked == null) return;
        if (hittedObject.Contains(other)) return;

        hittedObject.Add(other);

        attacked.OnHit(totalDamage);
    }
}
