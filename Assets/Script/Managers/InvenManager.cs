using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InvenManager
{
    List<SlotLine> slotLines = new List<SlotLine>();
    public int slotRowSize = 5;
    public int slotColumnSize = 9;
    private const string INVENTORY_PATH = "Canvas/InvenPanel/ItemArea/Content";
    Transform inventoryParent;

    public void OnStart()
    {
        inventoryParent = GameObject.Find(INVENTORY_PATH).transform;
        for(int y = 0; y < slotRowSize; y++)
        {
            if (inventoryParent.Find("SlotLine" + y).TryGetComponent(out SlotLine _line))
                slotLines.Add(_line);
            else
                Debug.LogError("SlotLine Is Not Assigned");
            for(int x = 0; x < slotColumnSize; x++)
            {
                if (slotLines[y] == null)
                    Debug.LogError("Line Is Not Assigned");
                if (inventoryParent.Find("SlotLine" + y + "/Slot" + x).TryGetComponent(out Slot _slot))
                    slotLines[y].mySlots.Add(_slot);
            }
        }
    }
    public IEnumerator MoveItem()
    {
        if(inventoryParent == null)
        {
            Debug.LogError("Inventory Is Not Assigned");
            yield return null;
        }
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);
            Slot targetSlot = null;
            if (raycastResults.Count > 0)
            {
                foreach (var go in raycastResults)
                {
                    if(go.gameObject.TryGetComponent(out Slot _slot))
                        targetSlot = _slot;
                }
            }
            while(true)
            {
                if (targetSlot == null)
                    break;
                Image targetImage = targetSlot.transform.Find("Item").GetComponent<Image>();
                targetImage.transform.position = Input.mousePosition;
                yield return null;
                if (Input.GetMouseButtonUp(0))
                {
                    pointer.position = Input.mousePosition;
                    raycastResults = new List<RaycastResult>(); // ����Ʈ �ʱ�ȭ
                    EventSystem.current.RaycastAll(pointer, raycastResults);
                    Slot destSlot = null;
                    if (raycastResults.Count > 0)
                    {
                        foreach (var go in raycastResults)
                        {
                            if (go.gameObject.TryGetComponent(out Slot _slot))
                                destSlot = _slot;
                        }
                    }
                    if(destSlot != null)
                    {
                        // ������ �̵�
                        int x = -1;
                        for (int y = 0; y < slotRowSize; y++)
                        {
                            if (slotLines[y].mySlots.Contains(destSlot))
                                x = slotLines[y].mySlots.FindIndex(_ => _.Equals(destSlot));
                        }
                        if (x < 0) // ������ Slot�� SlotLine ����Ʈ�� ���� ���
                            Debug.LogError("DestSlot Is Not Exist In SlotLine");
                        else
                        {
                            // ������ ���� ������
                        }
                    }
                    // �����̴� ��ó���� ���̰� �ϴ� ���� ��ǥ�̱� ������ ���� �̵��Ȱ� ���ڸ�
                    targetImage.transform.position = Vector3.zero;
                }
                break;
            }
        }
        Manager.Instance.invenCoroutine = null;
    }
}
