using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonSlot
{
    public bool emptyFlag = true; // ������ ������ �������
    public bool mainSlotFlag = false; // �������� ���� ���� ��ĭ, �� ������ ������ ����� ĭ����

    // �ش� �������� ������ ����� ĭ�� ������ ��� ����
    // �������� ���� �� �����Ƿ� �ʱⰪ�� ������ ����, x��ǥ���� ������ y ��ǥ(Row)��
    public Vector2Int itemDataPos = -Vector2Int.one;
    public JsonItem slotItem;
    public JsonSlot SlotToJsonSlot(Slot _slot)
    {
        emptyFlag = _slot.emptyFlag;
        mainSlotFlag = _slot.mainSlotFlag;
        itemDataPos = _slot.itemDataPos;
        if(_slot.slotItem != null)
            slotItem = _slot.slotItem.ItemToJsonItem();
        return this;
    }
}