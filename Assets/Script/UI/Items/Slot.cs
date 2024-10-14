using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public bool emptyFlag = true; // ������ ������ �������
    public bool mainSlotFlag = false; // �������� ���� ���� ��ĭ, �� ������ ������ ����� ĭ����

    // �ش� �������� ������ ����� ĭ�� ������ ��� ����
    // �������� ���� �� �����Ƿ� �ʱⰪ�� ������ ����, x��ǥ���� ������ y ��ǥ(Row)��
    public Vector2Int itemDataPos = -Vector2Int.one;
    // �ش� ������ ������ �ִ� ������
    public Item slotItem;
    public GameObject itemVisual;
    public int itemIndex;

    public void SlotReset()
    {
        emptyFlag = true;
        mainSlotFlag = false;
        itemDataPos = -Vector2Int.one;
        slotItem = null;
        itemVisual = null;
        itemIndex = -1;
        Manager.Data.PlayerDataExport();
    }
    public void SlotCopy(Slot _slot, Vector2Int _itemDataPos)
    {
        emptyFlag = _slot.emptyFlag;
        mainSlotFlag = _slot.mainSlotFlag;
        itemDataPos = _itemDataPos;
        if(_slot.slotItem != null)
            slotItem = _slot.slotItem.ItemDeepCopy();
        else
            slotItem = _slot.slotItem;
        itemIndex = _slot.itemIndex;
        itemVisual = _slot.itemVisual;
    }
    public void JsonSlotToSlot(JsonSlot _jsonSlot)
    {
        emptyFlag = _jsonSlot.emptyFlag;
        mainSlotFlag = _jsonSlot.mainSlotFlag;
        itemDataPos = _jsonSlot.itemDataPos;
        itemIndex = _jsonSlot.itemIndex;
    }
}
