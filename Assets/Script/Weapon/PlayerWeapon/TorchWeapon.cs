using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchWeapon : Weapon
{
    public override void SubAttackStart()
    {
        StartCoroutine(ThrowTorch());
    }

    IEnumerator ThrowTorch()
    {
        yield return new WaitForSeconds(0.8f);
        //던진 후 빈손으로
        Global.PlayerWeapon = null;
        //사용 후 인벤에서 삭제
        Manager.Inven.DeleteBoxItem
            (Manager.Inven.equipSlots[ItemType.Consumable.ToString() + Manager.Input.currentUtilitySlot], ItemBoxType.Equip);
        Manager.Input.currentUtilitySlot = -1;
        Manager.Inven.MainEquipUIAlpha();

        transform.parent = null;

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.AddForce(Camera.main.transform.forward * 10f, ForceMode.Impulse);

        Collider col = GetComponent<Collider>();
        col.enabled = true;
        col.isTrigger = false;
        gameObject.layer = LayerMask.NameToLayer("Interactive");

        yield return new WaitForSeconds(1f);

        Global.PlayerWeaponEquip(null);
        Destroy(this);
    }
}
