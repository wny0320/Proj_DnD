using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : EnemyWeapon
{
    [SerializeField]
    GameObject arrow;

    public void AttackStart(Transform trans)
    {
        GameObject go = Instantiate(arrow);
        go.transform.position = transform.position;

        Rigidbody rb = go.GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.AddForce(trans.forward * 20f, ForceMode.VelocityChange);
    }
}
