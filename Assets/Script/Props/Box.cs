using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Box : MonoBehaviour, IReceiveAttack
{
    [SerializeField] int hp = 2;

    public void OnHit(float damage)
    {
        hp--;
        if(hp <= 0)
        {
            Destroy(gameObject);
            Global.sfx.Play(Global.Sound.BoxBreak, transform.position);
            List<Item> dropItemList = Manager.Inven.GetRandomItem(0, 2);
            foreach (Item item in dropItemList)
            {
                GameObject dropItem3D = Instantiate(Manager.Data.item3DPrefab[item.itemIndex]);
                Item newItem = item.ItemDeepCopy();
                dropItem3D.GetComponent<Item3D>().myItem = newItem;
                dropItem3D.transform.position = transform.position;
                dropItem3D.transform.position += new Vector3(0, 0.1f, 0);
            }
        }
    }
}
