using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHand : MonoBehaviour
{
    float damage = 0f;
    protected List<IReceiveAttack> hittedObject = new();

    private void OnEnable()
    {
        hittedObject.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        damage = Manager.Game.Player.GetComponent<PlayerController>().stat.Attack;
        IReceiveAttack attacked = GetHittedObj(other.transform);
        if (attacked == null) return;
        if (hittedObject.Contains(attacked)) return;
        hittedObject.Add(attacked);

        attacked.OnHit(damage);
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
