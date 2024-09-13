using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InvenManager
{
    List<SlotLine> slotLines = new List<SlotLine>();
    Dictionary<string, Slot> equipSlots = new Dictionary<string, Slot>();
    public int slotRowSize = 5;
    public int slotColumnSize = 9;
    public int itemMaxSize = 4; // 2^4 X 2^4 짜리가 최대 크기라고 가정
    private const int UNITSIZE = 90;
    Vector2 invenStandardPos = new Vector2(40, -40);
    private const string INVENTORY_PATH = "InvenCanvas/InvenPanel/ItemArea/Content";
    private const string EQUIP_PATH = "InvenCanvas/InvenPanel/EquipArea/Slots";
    private const string EQUIP_VISUAL_PATH = "InvenCanvas/InvenPanel/EquipArea/ItemVisual";
    private const string INVENTORY_VISUAL_PATH = "InvenCanvas/InvenPanel/ItemArea/ItemVisual";
    private const string EQUIP_UI_PATH = "GameUI/EquipUI/Slot";
    private const string ITEM_UI_TAG = "ItemUI";
    private const string INVENTORY_SLOT_TAG = "InvenSlot";
    private const string EQUIP_SLOT_TAG = "EquipSlot";
    Transform inventoryParent;
    Canvas invenCanvas;
    CanvasGroup invenCanvasGroup;
    bool canvasVisualFlag;

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
        invenCanvas = GameObject.Find("InvenCanvas").GetComponent<Canvas>();
        invenCanvasGroup = invenCanvas.GetComponent<CanvasGroup>();
        invenCanvasGroup.alpha = 0f;
        invenCanvasGroup.interactable = false;
        canvasVisualFlag = false;

        foreach(var part in Enum.GetValues(typeof(EquipPart)))
        {
            string partsName = part.ToString();
            if (part.Equals(EquipPart.Weapon))
            {
                equipSlots.Add(partsName + 1, GameObject.Find(EQUIP_PATH + "/" + partsName + 1).GetComponent<Slot>());
                equipSlots.Add(partsName + 2, GameObject.Find(EQUIP_PATH + "/" + partsName + 2).GetComponent<Slot>());
            }
            else
                equipSlots.Add(partsName, GameObject.Find(EQUIP_PATH + "/" + partsName).GetComponent<Slot>());
        }
    }
    public void OnUpdate()
    {
        if (invenCanvas == null)
            return;
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (canvasVisualFlag == true)
            {
                canvasVisualFlag = false;
                invenCanvasGroup.alpha = 0f;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                canvasVisualFlag = true;
                invenCanvasGroup.alpha = 1f;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
    public Vector2 InvenPosCal(Vector2 _originPos, Vector2 _slotIndex)
    {
        return _originPos + new Vector2 (_slotIndex.y, -_slotIndex.x) * UNITSIZE;
    }
    // 고쳐야할 점, slot안에 itemImage가 있는 경우 Slot 안에서만 보임 << 다시보니 아닌거 같은데? 걍 크게 하면 문제 없을듯? << 따로 visual을 만들어서 해결
    public IEnumerator ItemManage()
    {
        while (true)
        { 
            yield return null;
            if(inventoryParent == null)
            {
                Debug.LogError("Inventory Is Not Assigned");
                continue;
            }
            if(canvasVisualFlag == false)
            {
                Debug.Log("Canvas Is Invisible");
                continue;
            }
            int mousebutton = -1;
            if (Input.GetMouseButtonDown(0))
            {
                mousebutton = 0;
            }
            else if(Input.GetMouseButtonDown(1))
            {
                mousebutton = 1;
            }
            if (mousebutton > -1)
            {
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position = Input.mousePosition;

                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, raycastResults);
                Slot targetSlot = null;
                GameObject itemVisual = null;

                // 장비창 slot에 상호작용하는 경우 활성화되는 플래그
                bool interactEquipFlag = false;
                if (raycastResults.Count > 0)
                {
                    foreach (var go in raycastResults)
                    {
                        Debug.Log(go);
                        if(go.gameObject.CompareTag(ITEM_UI_TAG) == true)
                        {
                            itemVisual = go.gameObject;
                            continue;
                        }
                        if(go.gameObject.TryGetComponent(out Slot _slot))
                        {
                            // 인벤토리 슬롯인 경우
                            if (go.gameObject.CompareTag(INVENTORY_SLOT_TAG) == true)
                                interactEquipFlag = false;
                            // 장비창(장착) 슬롯인 경우
                            if(go.gameObject.CompareTag(EQUIP_SLOT_TAG) == true)
                                interactEquipFlag = true;

                            targetSlot = _slot;
                        }
                    }
                }
                if(targetSlot != null && itemVisual != null && targetSlot.CompareTag(EQUIP_SLOT_TAG) == false)
                {
                    // 클릭된 슬롯이 메인 슬롯이 아닐 경우, 메인슬롯의 아이템 데이터로 치환
                    if (targetSlot.mainSlotFlag == false)
                    {
                        Vector2Int index = targetSlot.itemDataPos;
                        Debug.Log("index = " + index.y + ", " + index.x);
                        targetSlot = slotLines[index.x].mySlots[index.y];
                    }
                    //Debug.Log(targetSlot.slotItem.itemName);
                }
                Transform itemVisualTrans = null;
                Vector2 itemVisualOriginPos = Vector2.zero;
                if (itemVisual != null)
                {
                    itemVisualTrans = itemVisual.transform;
                    itemVisualOriginPos = itemVisualTrans.GetComponent<RectTransform>().anchoredPosition;
                    Debug.Log("Mouse Button Down");
                }
                #region MouseButtonLeft
                while (mousebutton == 0)
                {
                    if (targetSlot == null)
                        break;
                    if (itemVisual == null)
                        break;
                    // 아이템 크기 때문에 배경이 보이게 설정
                    itemVisual.GetComponent<Image>().color = new Color32(255, 255, 255, 10);
                    // 이부분이 지금 문제, 위치가 마음대로 이동함 -> anchoredPosition과 ScreenPointToLocalPointInRectangle을 통해 해결함
                    Vector2 convertedMousePos;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle
                        (invenCanvas.transform as RectTransform, Input.mousePosition, invenCanvas.worldCamera, out convertedMousePos);
                    itemVisualTrans.position = invenCanvas.transform.TransformPoint(convertedMousePos);
                    #region 과거코드
                    //// 마우스 위치 레이케이스 반복
                    //pointer.position = Input.mousePosition;
                    //raycastResults.Clear();
                    //EventSystem.current.RaycastAll(pointer, raycastResults);
                    //Slot overlayedSlot = null;
                    //GameObject overlayedItemVisual = null;
                    //if (raycastResults.Count > 0)
                    //{
                    //    foreach (var go in raycastResults)
                    //    {
                    //        if(go.gameObject.CompareTag(ITEM_UI_TAG) == true)
                    //        {
                    //            overlayedItemVisual = go.gameObject;
                    //            continue;
                    //        }
                    //        if (go.gameObject.TryGetComponent(out Slot _slot))
                    //        {
                    //            if(_slot.Equals(targetSlot) == false)
                    //            {
                    //                overlayedSlot = _slot;
                    //            }
                    //        }
                    //    }
                    //}
                    //Debug.Log("overlayedSlot = " + overlayedSlot);
                    //if(overlayedSlot != null)
                    //{
                    //    if(overlayedSlot.mainSlotFlag == false)
                    //    {
                    //        // 메인 슬롯이 아니면 메인슬롯 탐색
                    //    }
                    //}
                    //overlayedItemVisualBefore = overlayedItemVisual;
                    //overlayedSlotBefore = overlayedSlot;

                    //itemVisualRectTrans.position = Input.mousePosition;
                    //yield return null;
                    #endregion
                    if (Input.GetMouseButtonUp(0))
                    {
                        itemVisual.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                        Vector2 itemVisualPos = itemVisual.GetComponent<RectTransform>().anchoredPosition;

                        // 마우스의 위치가 가운데에 위치한 경우 가까운 위치로 지정하기 위한 코드
                        float targetXPos = itemVisualPos.x;
                        // y 좌표는 양수가 아닌 음수로 늘어남
                        float targetYPos = itemVisualPos.y;
                        // 아이템이 들어갈 위치
                        Vector2 targetPos = new Vector2(targetXPos, targetYPos);
                        int[] itemSize = GetItemSize(targetSlot.slotItem);
                        int xIndex = 
                            Mathf.RoundToInt(((targetXPos - (itemSize[1] - 1) * (UNITSIZE / 2)) - invenStandardPos.x) / UNITSIZE);
                        int yIndex = 
                            Mathf.RoundToInt(((targetYPos + (itemSize[0] - 1) * (UNITSIZE / 2)) - invenStandardPos.y) / -UNITSIZE);
                        int originXIndex = 
                            Mathf.RoundToInt(((itemVisualOriginPos.x - (itemSize[1] - 1) * (UNITSIZE / 2)) - invenStandardPos.x) / UNITSIZE);
                        int originYIndex = 
                            Mathf.RoundToInt(((Mathf.Abs(itemVisualOriginPos.y) - (itemSize[0] - 1) * (UNITSIZE / 2)) 
                            - Mathf.Abs(invenStandardPos.y)) / UNITSIZE);
                        bool breakFlag = false;
                        // 옮길 수 있는지 판별
                        for (int j = 0; j < itemSize[1]; j++)
                        {
                            if (xIndex + itemSize[1] > slotColumnSize || yIndex + itemSize[0] > slotRowSize || xIndex < 0 || yIndex < 0)
                            {
                                breakFlag = true;
                                break;
                            }
                            for (int i = 0; i < itemSize[0]; i++)
                            {
                                Slot to = slotLines[yIndex + i].mySlots[xIndex + j];
                                if (to.emptyFlag == true)
                                    continue;
                                else
                                {
                                    Debug.LogError("Already Item Is Been There. Can't Move Here.");
                                    breakFlag = true;
                                }
                            }
                        }
                        //옮길 수 없는 경우들
                        if (breakFlag == true)
                        {
                            // 아이템 버리기
                            if(xIndex < -1) // 인게임이라는 조건을 추가해야함. 나중에 씬 정해지면 하면 될 듯?
                            {
                                // 아이템 버리기
                                DumpItem(slotLines[originYIndex].mySlots[originXIndex].slotItem);
                                DeleteInvenItem(new Vector2Int(originYIndex, originXIndex));
                            }
                            // 아이템 옮길 수 없음
                            else
                            {
                                Debug.LogError("Item Moved Unproper Position");
                                itemVisual.GetComponent<RectTransform>().anchoredPosition = itemVisualOriginPos;
                            }
                            break;
                        }
                        // 옮길 수 있는 경우
                        Vector2 itemPos = Vector2.zero;
                        Debug.Log(invenStandardPos.y);
                        itemVisual.GetComponent<RectTransform>().anchoredPosition = 
                            new Vector2(invenStandardPos.x + (xIndex + (float)(itemSize[1] - 1) / 2) * UNITSIZE,
                            invenStandardPos.y - (yIndex + (float)(itemSize[0] - 1) / 2) * UNITSIZE);
                        Debug.Log("yIndex, xIndex = " + yIndex + ", " + xIndex);
                        for (int j = 0; j < itemSize[0]; j++)
                        {
                            for(int i = 0; i < itemSize[1]; i++)
                            {
                                Debug.Log("ji = " + j + ", " + i);
                                Debug.Log("originYIndex, originXindex = " + originYIndex + ", " + originXIndex);
                                Slot from = slotLines[originYIndex + j].mySlots[originXIndex + i];
                                Slot to = slotLines[yIndex + j].mySlots[xIndex + i];
                                to.SlotCopy(from, new Vector2Int(yIndex, xIndex));
                                from.SlotReset();
                            }
                        }
                        Debug.Log("Item Moved");
                        Debug.Log("Mouse Button Up");
                        break;
                        #region 과거코드
                        //xIndex = Mathf.RoundToInt(xIndex);
                        //yIndex = Mathf.RoundToInt(yIndex);
                        //Debug.Log(xIndex + ", " + yIndex);

                        //pointer.position = Input.mousePosition;
                        //raycastResults = new List<RaycastResult>(); // 리스트 초기화
                        //EventSystem.current.RaycastAll(pointer, raycastResults);
                        //Slot destSlot = null;
                        //if (raycastResults.Count > 0)
                        //{
                        //    foreach (var go in raycastResults)
                        //    {
                        //        if (go.gameObject.TryGetComponent(out Slot _slot))
                        //        {
                        //            if (_slot.emptyFlag == true)
                        //                destSlot = _slot;
                        //        }
                        //    }
                        //}
                        //if (destSlot == null)
                        //{
                        //    // 이동 실패시 움직이는 것처럼만 보이게 하는 것이 목표이기 때문에 실제 이동된건 제자리
                        //    itemVisualTrans.GetComponent<RectTransform>().anchoredPosition = itemVisualOriginPos;
                        //    Debug.LogError("Can't Move Item Here");
                        //}
                        //else
                        //{
                        //    //데이터와 itemVisual 옮기기
                        //    for (int y = targetSlot.itemDataPos.y; y < targetSlot.itemDataPos.y + itemSize[0]; y++)
                        //    {
                        //        for (int x = targetSlot.itemDataPos.x; x < targetSlot.itemDataPos.x + itemSize[1]; x++)
                        //        {
                        //            slotLines[y].mySlots[x].SlotReset();
                        //        }
                        //    }
                        //    itemVisualTrans.GetComponent<RectTransform>().anchoredPosition = targetPos;
                        //    Debug.Log("ItemMove");
                        //}
                        //Debug.Log("Mouse Button Up");
                        //break;
                        #endregion
                    }
                    yield return null;
                }
                #endregion
                #region MouseButtonRight
                while (mousebutton == 1)
                {
                    if (targetSlot == null)
                        break;
                    if (itemVisual == null)
                        break;
                    // 장착된 장비가 드래그 되는 경우는 추후 생각

                    // 장비창에서 해제하는 경우
                    // targetSlot이 EquipSlot일 경우
                    if(interactEquipFlag == true)
                    {
                        foreach(var part in Enum.GetValues(typeof(EquipPart)))
                        {
                            // 1번칸 무기인경우
                            string targetImagePath;
                            if (part.ToString() + 1 == targetSlot.gameObject.name.ToString())
                                targetImagePath = EQUIP_UI_PATH + 1 + "/ItemImage";
                            // 2번칸 무기인경우
                            else if (part.ToString() + 2 == targetSlot.gameObject.name.ToString())
                                targetImagePath = EQUIP_UI_PATH + 2 + "/ItemImage";
                            else
                                targetImagePath = null;
                            if(targetImagePath != null)
                                GameObject.Find(targetImagePath).GetComponent<Image>().sprite = null;
                            Debug.Log(targetImagePath);
                        }
                        // AddItem이 실패한경우 장착한 아이템을 바닥에 버림
                        Debug.Log(targetSlot.gameObject.name.ToString());
                        Debug.Log(targetSlot.slotItem);
                        if(AddItem(targetSlot.slotItem) == false)
                            DumpItem(targetSlot.slotItem);
                        // AddItem이 성공한 경우는 그냥 장비창 리셋만 하면 됨
                        GameObject.Destroy(itemVisual);
                        targetSlot.SlotReset();
                        break;
                    }
                    // 인벤토리창에서 장착하는 경우
                    else
                    {
                        EquipPart targetEquipPart = targetSlot.slotItem.equipPart;
                        ItemType targetItemType = targetSlot.slotItem.itemType;
                        if (targetItemType != ItemType.Equipment)
                        {
                            Debug.LogError("Target Item Is Not Equipment");
                            break;
                        }
                        string equipPartsName = null;
                        Slot equipSlot = null;
                        if (targetEquipPart != EquipPart.Weapon)
                        {
                            equipPartsName = targetEquipPart.ToString();
                            equipSlot = equipSlots[equipPartsName];
                        }
                        if (targetEquipPart == EquipPart.Weapon)
                        {
                            string WeaponPartsName = targetEquipPart.ToString();
                            Slot WeaponSlot1 = equipSlots[WeaponPartsName + 1];
                            Slot WeaponSlot2 = equipSlots[WeaponPartsName + 2];
                            // 무기 1슬롯이 빈 상황
                            if (WeaponSlot1.emptyFlag == true)
                            {
                                equipSlot = WeaponSlot1;
                                equipPartsName = WeaponPartsName + 1;
                            }
                            // 그 외는 2번 슬롯, 2번 슬롯이 어차피 비었는지 안비었는지는 아래에서 또 확인할거라 패스함
                            else
                            {
                                equipSlot = WeaponSlot2;
                                equipPartsName = WeaponPartsName + 2;
                            }
                        }
                        Vector2 itemVisualPos = itemVisual.GetComponent<RectTransform>().anchoredPosition;

                        // 마우스의 위치가 가운데에 위치한 경우 가까운 위치로 지정하기 위한 코드
                        float targetXPos = itemVisualPos.x;
                        // y 좌표는 양수가 아닌 음수로 늘어남
                        float targetYPos = itemVisualPos.y;
                        // 아이템이 들어갈 위치
                        Vector2 targetPos = new Vector2(targetXPos, targetYPos);
                        int[] itemSize = GetItemSize(targetSlot.slotItem);
                        int xIndex =
                            Mathf.RoundToInt(((targetXPos - (itemSize[1] - 1) * (UNITSIZE / 2)) - invenStandardPos.x) / UNITSIZE);
                        int yIndex =
                            Mathf.RoundToInt(((targetYPos + (itemSize[0] - 1) * (UNITSIZE / 2)) - invenStandardPos.y) / -UNITSIZE);
                        int originXIndex =
                            Mathf.RoundToInt(((itemVisualOriginPos.x - (itemSize[1] - 1) * (UNITSIZE / 2)) - invenStandardPos.x) / UNITSIZE);
                        int originYIndex =
                            Mathf.RoundToInt(((Mathf.Abs(itemVisualOriginPos.y) - (itemSize[0] - 1) * (UNITSIZE / 2))
                            - Mathf.Abs(invenStandardPos.y)) / UNITSIZE);
                        if (equipSlot.emptyFlag == true)
                        {
                            equipSlot.slotItem = targetSlot.slotItem;
                            equipSlot.emptyFlag = false;
                            // itemvisual 사이즈와 위치 동일시 시키기
                            GameObject newVisual = GameObject.Instantiate(itemVisual);
                            newVisual.transform.SetParent(GameObject.Find(EQUIP_VISUAL_PATH).transform);
                            equipSlot.itemVisual = newVisual;
                            RectTransform equipRectTrans = equipSlot.gameObject.GetComponent<RectTransform>();
                            RectTransform equipVisualRectTrans = newVisual.GetComponent<RectTransform>();
                            equipVisualRectTrans.anchorMin = new Vector2(0, 0);
                            equipVisualRectTrans.anchorMax = new Vector2(0, 0);
                            equipVisualRectTrans.anchoredPosition = equipRectTrans.anchoredPosition;
                            equipVisualRectTrans.sizeDelta = equipRectTrans.sizeDelta;
                            // 무기인경우 UI 동기화
                                string targetImagePath = null;
                            if(targetEquipPart == EquipPart.Weapon)
                            {
                                if (equipPartsName == targetEquipPart.ToString() + 1)
                                    targetImagePath = EQUIP_UI_PATH + 1 + "/ItemImage";
                                else
                                    targetImagePath = EQUIP_UI_PATH + 2 + "/ItemImage";
                                GameObject.Find(targetImagePath).GetComponent<Image>().sprite = 
                                    itemVisual.transform.GetChild(0).GetComponent<Image>().sprite;
                            }
                            DeleteInvenItem(new Vector2Int(originYIndex, originXIndex));
                            Debug.Log(GameObject.Find(targetImagePath).GetComponent<Image>().sprite);
                            break;
                        }
                        else
                        {
                            Debug.LogError("Target Equip Slot Is Not Empty.");
                            break;
                        }
                    }
                }
                #endregion
            }
        }
        //Manager.Instance.invenCoroutine = null;
    }
    public bool AddItem(Item _item)
    {
        // 비어있는 슬롯을 다 가져오는 코드
        bool[,] emptyFlag = new bool[slotRowSize,slotColumnSize];
        int[] itemSize = GetItemSize(_item);
        List<Slot> targetedSlotList = new List<Slot>(); // 탐색을 이미 진행한 슬롯의 리스트
        Slot targetSlot = null; // 타겟이 된 슬롯
        Vector2Int targetSlotIndex = new Vector2Int(0,0); // 타켓 슬롯의 인덱스 위치, y x
        Vector2 targetSlotPos = Vector2.zero;

        bool searchSuccessFlag = false; // 빈 공간을 찾기 위한 탐색 플래그
        bool findTargetFlag; // 타겟 슬롯을 찾으면 true가 되는 플래그

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
                Debug.Log("AllEmptySlot = (" + y + ", " + x + ")");
            }
        }
        // 타겟 슬롯을 찾을때까지 반복
        while (searchSuccessFlag == false)
        {
            // 탐색 관련 변수들 초기화
            findTargetFlag = false;
            // 비어있는 슬롯 하나를 찾는 코드
            for (int y = 0;y < slotRowSize;y++)
            {
                if (findTargetFlag == true)
                    break;
                for(int x = 0;x < slotColumnSize;x++)
                {
                    if (emptyFlag[y, x] == true)
                    {
                        targetSlot = slotLines[y].mySlots[x];
                        targetSlotIndex.x = y;
                        targetSlotIndex.y = x;
                        targetSlotPos = targetSlot.GetComponent<RectTransform>().position;
                        if (targetedSlotList.Contains(targetSlot))
                        {
                            targetSlot = null;
                            targetSlotIndex = new Vector2Int(0, 0);
                            continue;
                        }
                        else
                        {
                            findTargetFlag = true;
                            targetedSlotList.Add(targetSlot);
                            break;
                        }
                    }
                }
            }
            if (targetSlot == null)
                break;
            bool re_searchFlag = false; // 빈 슬롯을 다시 탐색하는 것을 나타내는 플래그
            // 탐색해야하는 슬롯이(아이템의 크기가) 최대 크기(가방크기)보다 큰 경우 다음 탐색으로
            if (itemSize[0] + targetSlotIndex.x > slotRowSize || itemSize[1] + targetSlotIndex.y > slotColumnSize)
                continue;

            // 비어 있는 슬롯 주변을 다시 탐색하는 코드
            // 탐색에 성공한다면 다음 탐색 진행
            // 탐색에 실패한다면 break해서 다시 위 코드로
            for (int y = targetSlotIndex.x; y < itemSize[0] + targetSlotIndex.x; y++)
            {
                if (re_searchFlag == true)
                    break;
                for (int x = targetSlotIndex.y; x < itemSize[1] + targetSlotIndex.y; x++)
                {
                    if (emptyFlag[y, x] == true)
                        continue;
                    else
                    {
                        re_searchFlag = true;
                        break;
                    }
                }
            }
            if (re_searchFlag == true)
                continue;
            else
                searchSuccessFlag = true;
        }
        if(targetSlot == null)
        {
            Debug.LogError("Empty Slot Is Not Exist");
            return false;
        }
        Debug.Log("TargetSlotPos = " + targetSlotIndex);
        targetSlot.emptyFlag = false;
        targetSlot.mainSlotFlag = true;
        targetSlot.itemDataPos = targetSlotIndex;
        targetSlot.slotItem = _item.ItemDeepCopy();
        if(targetSlot.slotItem.randomStatFlag == false)
        {
            targetSlot.slotItem.ItemRandomStat();
            targetSlot.slotItem.randomStatFlag = true;
        }
        Debug.Log("ItemSize = " + itemSize[0] + ", " + itemSize[1]);
        int minY = targetSlotIndex.x;
        int maxY = targetSlotIndex.x + itemSize[0];
        int minX = targetSlotIndex.y;
        int maxX = targetSlotIndex.y + itemSize[1];
        Debug.Log("min/max = " + maxY + ", " + minY + ", " + maxX + ", " + minX);
        // 아이템 정보 넣기
        for (int y = minY; y < maxY; y++)
        {
            for (int x = minX; x < maxX; x++)
            {
                Debug.Log("Filled Slot Pos = (" + y + ", " + x + ")");
                Slot nowSlot = slotLines[y].mySlots[x];
                if (targetSlot.Equals(nowSlot))
                    nowSlot.mainSlotFlag = true;
                else
                    nowSlot.mainSlotFlag = false;
                nowSlot.emptyFlag = false;
                nowSlot.itemDataPos = targetSlotIndex;
                // 우선 헷갈리지 않게 그냥 칸에 같은 이미지 박는걸로 해놨음
                //if (nowSlot.transform.Find("Item/ItemImage").TryGetComponent<Image>(out Image _itemImage))
                //{
                //    _itemImage.color = new Color32(255, 255, 255, 255);
                //    _itemImage.sprite = Manager.Data.itemSprite[_item.itemImageNum];
                //}
                //else
                //    Debug.LogError("Can't Get Inventory Item Image Component");
            }
        }
        // 눈에 보이는 아이템 프리팹 생성
        GameObject itemVisual = GameObject.Instantiate(Manager.Data.itemUIPrefab[_item.itemIndex], GameObject.Find(INVENTORY_VISUAL_PATH).transform);
        itemVisual.transform.localPosition = InvenPosCal(itemVisual.transform.localPosition, targetSlotIndex);
        // itemVisual 정보 저장
        targetSlot.itemVisual = itemVisual;
        return true;
    }
    public void DeleteInvenItem(Vector2Int _targetMainSlotIndex)
    {
        Slot targetSlot = slotLines[_targetMainSlotIndex.x].mySlots[_targetMainSlotIndex.y];
        int[] itemSize = GetItemSize(targetSlot.slotItem);
        GameObject.Destroy(targetSlot.itemVisual);
        for(int y = 0;  y < itemSize[1]; y++)
        {
            for(int x = 0; x < itemSize[0]; x++)
            {
                Slot nowSlot = slotLines[_targetMainSlotIndex.x + x].mySlots[_targetMainSlotIndex.y + y];
                nowSlot.SlotReset();
                Debug.Log((_targetMainSlotIndex.x + x) + ", " + (_targetMainSlotIndex.y + y) + " Slot Delete Data");
            }
        }
    }
    public void DumpItem(Item _item)
    {
        Debug.Log("Item Dumped");
        GameObject dumpedItem3D = GameObject.Instantiate(Manager.Data.item3DPrefab[_item.itemIndex]);
        Item newItem = _item.ItemDeepCopy();
        dumpedItem3D.GetComponent<Item3D>().myItem = newItem;
        dumpedItem3D.transform.position = Manager.Game.Player.transform.position;
        dumpedItem3D.transform.position += new Vector3(0, 0.1f, 0);
    }
    //public void ImageRefreshItem()
    //{
    //    for(int y = 0; y < slotRowSize; y++)
    //    {
    //        for(int x = 0; x < slotColumnSize; x++)
    //        {
    //            Slot targetSlot = slotLines[y].mySlots[x];
    //            Image targetImage = targetSlot.transform.Find("Item").GetComponent<Image>();
    //            if(targetSlot.mainSlotFlag == false)
    //            {
    //                targetImage.color = new Color32(255, 255, 255, 0);
    //            }
    //            else
    //            {
    //                targetImage.color = new Color32(255, 255, 255, 255);
    //            }
    //        }
    //    }
    //}
    public int[] GetItemSize(Item _item) // Enum에 있는 값을 실제 아이템 사이즈 값으로 반환하는 코드, 데이터 매니저로 나중에 옮기든 할듯
    {
        byte targetSize = (byte)_item.itemSize;
        byte ySize = (byte)(targetSize >> itemMaxSize); // 앞 4비트 확인
        byte xSize = (byte)(targetSize - (ySize << itemMaxSize)); // 뒤 4비트 확인
        int[] convertedSize = {ySize,xSize};
        return convertedSize;
    }
}
