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
    public int itemMaxSize = 4; // 2^4 X 2^4 짜리가 최대 크기라고 가정
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
                    raycastResults = new List<RaycastResult>(); // 리스트 초기화
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
                        // 아이템 이동
                        int x = -1;
                        for (int y = 0; y < slotRowSize; y++)
                        {
                            if (slotLines[y].mySlots.Contains(destSlot))
                                x = slotLines[y].mySlots.FindIndex(_ => _.Equals(destSlot));
                        }
                        if (x < 0) // 목적지 Slot이 SlotLine 리스트에 없는 경우
                            Debug.LogError("DestSlot Is Not Exist In SlotLine");
                        else
                        {
                            // 아이템 정보 보내기
                        }
                    }
                    // 움직이는 것처럼만 보이게 하는 것이 목표이기 때문에 실제 이동된건 제자리
                    targetImage.transform.position = Vector3.zero;
                }
                break;
            }
        }
        Manager.Instance.invenCoroutine = null;
    }
    public void AddItem(Item _item)
    {
        // 어디가 비었는지 다시 불러들이는 코드
        bool[,] emptyFlag = new bool[slotRowSize,slotColumnSize];
        int[] itemSize = GetItemSize(_item);
        for(int y = 0; y < slotRowSize; y++)
        {
            if (slotLines[y].lineEmptyFlag == false) // 해당하는 라인이 꽉차있다면 패스
                continue;
            for (int x = 0; x < slotColumnSize; x++)
            {
                Slot nowSlot = slotLines[y].mySlots[x];
                if (nowSlot.emptyFlag == false) // 해당 슬롯이 꽉차있다면 패스
                    continue;
                emptyFlag[y, x] = true;
            }
        }

        Vector2Int emptyCnt = new Vector2Int(0,0); // 비어있는 칸의 갯수, y x
        Slot targetSlot = null;
        Vector2Int targetSlotPos = new Vector2Int(0,0); // 타켓 슬롯의 위치, y x
        bool searchSuccessFlag = false;
        for (int y = 0; y < slotRowSize; y++)
        {
            bool rowCal = false; // 해당 row를 계산에 포함했는지 알기 위한 변수
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
                else // 연속적으로 비어있지 않다면 초기화
                {
                    emptyCnt = new Vector2Int(0,0);
                    rowCal = false;
                    targetSlot = null;
                }
                if (itemSize[0] <= emptyCnt[0] && itemSize[1] <= emptyCnt[1]) // 빈 공간이 아이템의 크기보다 클 경우, 탐색을 멈추고 플래그를 반환
                {
                    searchSuccessFlag = true;
                    break;
                }
            }
        }
        if(searchSuccessFlag == true) // 아이템 크기만큼 빈 공간 탐색에 성공한 경우
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
            //아이템 이미지 넣기
            if(targetSlot.transform.Find("Item").TryGetComponent<Image>(out Image _itemImage))
            {
                _itemImage.sprite = Manager.Data.itemSprite[_item.itemImageNum];
            }
        }
        else // 아이템 크기만큼 빈 공간 탐색에 실패한 경우
        {
            Debug.LogError("Empty Slot Is Not Exist");
        }
    }
    public int[] GetItemSize(Item _item) // Enum에 있는 값을 실제 아이템 사이즈 값으로 반환하는 코드, 데이터 매니저로 나중에 옮기든 할듯
    {
        byte targetSize = (byte)_item.itemSize;
        byte ySize = (byte)(targetSize >> itemMaxSize); // 앞 4비트 확인
        byte xSize = (byte)(targetSize - (ySize << itemMaxSize)); // 뒤 4비트 확인
        int[] convertedSize = {ySize,xSize};
        return convertedSize;
    }
}
