using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    float damage = 30f;

    Collider col;
    Rigidbody rb;

    private void Start()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        Debug.Log(transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("AAAA");
        IReceiveAttack attacked = GetHittedObj(other.transform);
        StuckArrow(other.transform);
        if (attacked == null)
            return;

        attacked.OnHit(damage);
    }

    private void StuckArrow(Transform trans)
    {
        //transform.parent = trans.root;
        Destroy(rb);
        Destroy(col);
        Destroy(this);
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
