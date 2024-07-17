using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour, IReceiveAttack
{
    [SerializeField] int hp = 2;

    public void OnHit(float damage)
    {
        Debug.Log("AA");
        hp--;
        if(hp <= 0)
            Destroy(gameObject);
    }
}
