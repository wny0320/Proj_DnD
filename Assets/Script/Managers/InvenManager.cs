using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using JetBrains.Annotations;

public class InvenManager
{
    List<SlotLine> slotLines = new List<SlotLine>();
    public int slotRowSize = 5;
    public int slotColumnSize = 9;
    public int itemMaxSize = 4; // 2^4 X 2^4 ¥���� �ִ� ũ���� ����
    private const string INVENTORY_PATH = "InvenCanvas/InvenPanel/ItemArea/Content";
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
    public void AddItem(Item _item)
    {
        // ��� ������� �ٽ� �ҷ����̴� �ڵ�
        bool[,] emptyFlag = new bool[slotRowSize,slotColumnSize];
        int[] itemSize = GetItemSize(_item);
        for(int y = 0; y < slotRowSize; y++)
        {
            if (slotLines[y].lineEmptyFlag == false) // �ش��ϴ� ������ �����ִٸ� �н�
                continue;
            for (int x = 0; x < slotColumnSize; x++)
            {
                Slot nowSlot = slotLines[y].mySlots[x];
                if (nowSlot.emptyFlag == false) // �ش� ������ �����ִٸ� �н�
                    continue;
                emptyFlag[y, x] = true;
            }
        }

        Vector2Int emptyCnt = new Vector2Int(0,0); // ����ִ� ĭ�� ����, y x
        Slot targetSlot = null;
        Vector2Int targetSlotPos = new Vector2Int(0,0); // Ÿ�� ������ ��ġ, y x
        bool searchSuccessFlag = false;
        for (int y = 0; y < slotRowSize; y++)
        {
            bool rowCal = false; // �ش� row�� ��꿡 �����ߴ��� �˱� ���� ����
            for (int x = 0; x < slotColumnSize; x++)
            {
                if (emptyFlag[y, x] == true)
                {
                    if(rowCal == false)
                    {
                        emptyCnt[0]++;
                        rowCal = true;
                        targetSlot = slotLines[y].mySlots[x];
                        targetSlotPos.x = y;
                        targetSlotPos.y = x;
                    }
                    emptyCnt[1]++;
                }
                else // ���������� ������� �ʴٸ� �ʱ�ȭ
                {
                    emptyCnt = new Vector2Int(0,0);
                    rowCal = false;
                    targetSlot = null;
                }
                if (itemSize[0] <= emptyCnt[0] && itemSize[1] <= emptyCnt[1]) // �� ������ �������� ũ�⺸�� Ŭ ���, Ž���� ���߰� �÷��׸� ��ȯ
                {
                    searchSuccessFlag = true;
                    break;
                }
            }
        }
        if(searchSuccessFlag == true) // ������ ũ�⸸ŭ �� ���� Ž���� ������ ���
        {
            targetSlot.emptyFlag = false;
            targetSlot.mainSlotFlag = true;
            targetSlot.itemDataPos = targetSlotPos;
            targetSlot.slotItem = _item;
            int maxY = targetSlotPos.x + emptyCnt[0];
            int maxX = targetSlotPos.y + emptyCnt[1];
            for(int y = targetSlotPos.x; y < maxY; y++)
            {
                for(int x = targetSlotPos.y; x < maxX; x++)
                {
                    Slot nowSlot = slotLines[y].mySlots[x];
                    nowSlot.emptyFlag = false;
                    nowSlot.mainSlotFlag = false;
                    nowSlot.itemDataPos = targetSlotPos;
                }
            }
            //������ �̹��� �ֱ�
            if(targetSlot.transform.Find("Item").TryGetComponent<Image>(out Image _itemImage))
            {
                _itemImage.sprite = Manager.Data.itemSprite[_item.itemImageNum];
            }
        }
        else // ������ ũ�⸸ŭ �� ���� Ž���� ������ ���
        {
            Debug.LogError("Empty Slot Is Not Exist");
        }
    }
    public int[] GetItemSize(Item _item) // Enum�� �ִ� ���� ���� ������ ������ ������ ��ȯ�ϴ� �ڵ�, ������ �Ŵ����� ���߿� �ű�� �ҵ�
    {
        byte targetSize = (byte)_item.itemSize;
        byte ySize = (byte)(targetSize >> itemMaxSize); // �� 4��Ʈ Ȯ��
        byte xSize = (byte)(targetSize - (ySize << itemMaxSize)); // �� 4��Ʈ Ȯ��
        int[] convertedSize = {ySize,xSize};
        return convertedSize;
    }
}
