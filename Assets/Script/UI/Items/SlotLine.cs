using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotLine : MonoBehaviour
{
    public List<Slot> mySlots = new List<Slot>();
    public bool lineEmptyFlag; // 해당 줄이 비었는지 나타내는 bool
}
