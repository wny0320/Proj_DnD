using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipArea : MonoBehaviour
{
    public List<Slot> weaponList = new List<Slot>();
    public List<Slot> armorList = new List<Slot>();
    public List<Slot> consumList = new List<Slot>();
    public Transform itemVisualTrans;
}
