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
    public int itemMaxSize = 4; // 2^4 X 2^4 ¥���� �ִ� ũ���� ����
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
    // ���ľ��� ��, slot�ȿ� itemImage�� �ִ� ��� Slot �ȿ����� ���� << �ٽú��� �ƴѰ� ������? �� ũ�� �ϸ� ���� ������? << ���� visual�� ���� �ذ�
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

                // ���â slot�� ��ȣ�ۿ��ϴ� ��� Ȱ��ȭ�Ǵ� �÷���
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
                            // �κ��丮 ������ ���
                            if (go.gameObject.CompareTag(INVENTORY_SLOT_TAG) == true)
                                interactEquipFlag = false;
                            // ���â(����) ������ ���
                            if(go.gameObject.CompareTag(EQUIP_SLOT_TAG) == true)
                                interactEquipFlag = true;

                            targetSlot = _slot;
                        }
                    }
                }
                if(targetSlot != null && itemVisual != null && targetSlot.CompareTag(EQUIP_SLOT_TAG) == false)
                {
                    // Ŭ���� ������ ���� ������ �ƴ� ���, ���ν����� ������ �����ͷ� ġȯ
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
                    // ������ ũ�� ������ ����� ���̰� ����
                    itemVisual.GetComponent<Image>().color = new Color32(255, 255, 255, 10);
                    // �̺κ��� ���� ����, ��ġ�� ������� �̵��� -> anchoredPosition�� ScreenPointToLocalPointInRectangle�� ���� �ذ���
                    Vector2 convertedMousePos;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle
                        (invenCanvas.transform as RectTransform, Input.mousePosition, invenCanvas.worldCamera, out convertedMousePos);
                    itemVisualTrans.position = invenCanvas.transform.TransformPoint(convertedMousePos);
                    #region �����ڵ�
                    //// ���콺 ��ġ �������̽� �ݺ�
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
                    //        // ���� ������ �ƴϸ� ���ν��� Ž��
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

                        // ���콺�� ��ġ�� ����� ��ġ�� ��� ����� ��ġ�� �����ϱ� ���� �ڵ�
                        float targetXPos = itemVisualPos.x;
                        // y ��ǥ�� ����� �ƴ� ������ �þ
                        float targetYPos = itemVisualPos.y;
                        // �������� �� ��ġ
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
                        // �ű� �� �ִ��� �Ǻ�
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
                        //�ű� �� ���� ����
                        if (breakFlag == true)
                        {
                            // ������ ������
                            if(xIndex < -1) // �ΰ����̶�� ������ �߰��ؾ���. ���߿� �� �������� �ϸ� �� ��?
                            {
                                // ������ ������
                                DumpItem(slotLines[originYIndex].mySlots[originXIndex].slotItem);
                                DeleteInvenItem(new Vector2Int(originYIndex, originXIndex));
                            }
                            // ������ �ű� �� ����
                            else
                            {
                                Debug.LogError("Item Moved Unproper Position");
                                itemVisual.GetComponent<RectTransform>().anchoredPosition = itemVisualOriginPos;
                            }
                            break;
                        }
                        // �ű� �� �ִ� ���
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
                        #region �����ڵ�
                        //xIndex = Mathf.RoundToInt(xIndex);
                        //yIndex = Mathf.RoundToInt(yIndex);
                        //Debug.Log(xIndex + ", " + yIndex);

                        //pointer.position = Input.mousePosition;
                        //raycastResults = new List<RaycastResult>(); // ����Ʈ �ʱ�ȭ
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
                        //    // �̵� ���н� �����̴� ��ó���� ���̰� �ϴ� ���� ��ǥ�̱� ������ ���� �̵��Ȱ� ���ڸ�
                        //    itemVisualTrans.GetComponent<RectTransform>().anchoredPosition = itemVisualOriginPos;
                        //    Debug.LogError("Can't Move Item Here");
                        //}
                        //else
                        //{
                        //    //�����Ϳ� itemVisual �ű��
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
                    // ������ ��� �巡�� �Ǵ� ���� ���� ����

                    // ���â���� �����ϴ� ���
                    // targetSlot�� EquipSlot�� ���
                    if(interactEquipFlag == true)
                    {
                        foreach(var part in Enum.GetValues(typeof(EquipPart)))
                        {
                            // 1��ĭ �����ΰ��
                            string targetImagePath;
                            if (part.ToString() + 1 == targetSlot.gameObject.name.ToString())
                                targetImagePath = EQUIP_UI_PATH + 1 + "/ItemImage";
                            // 2��ĭ �����ΰ��
                            else if (part.ToString() + 2 == targetSlot.gameObject.name.ToString())
                                targetImagePath = EQUIP_UI_PATH + 2 + "/ItemImage";
                            else
                                targetImagePath = null;
                            if(targetImagePath != null)
                                GameObject.Find(targetImagePath).GetComponent<Image>().sprite = null;
                            Debug.Log(targetImagePath);
                        }
                        // AddItem�� �����Ѱ�� ������ �������� �ٴڿ� ����
                        Debug.Log(targetSlot.gameObject.name.ToString());
                        Debug.Log(targetSlot.slotItem);
                        if(AddItem(targetSlot.slotItem) == false)
                            DumpItem(targetSlot.slotItem);
                        // AddItem�� ������ ���� �׳� ���â ���¸� �ϸ� ��
                        GameObject.Destroy(itemVisual);
                        targetSlot.SlotReset();
                        break;
                    }
                    // �κ��丮â���� �����ϴ� ���
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
                            // ���� 1������ �� ��Ȳ
                            if (WeaponSlot1.emptyFlag == true)
                            {
                                equipSlot = WeaponSlot1;
                                equipPartsName = WeaponPartsName + 1;
                            }
                            // �� �ܴ� 2�� ����, 2�� ������ ������ ������� �Ⱥ�������� �Ʒ����� �� Ȯ���ҰŶ� �н���
                            else
                            {
                                equipSlot = WeaponSlot2;
                                equipPartsName = WeaponPartsName + 2;
                            }
                        }
                        Vector2 itemVisualPos = itemVisual.GetComponent<RectTransform>().anchoredPosition;

                        // ���콺�� ��ġ�� ����� ��ġ�� ��� ����� ��ġ�� �����ϱ� ���� �ڵ�
                        float targetXPos = itemVisualPos.x;
                        // y ��ǥ�� ����� �ƴ� ������ �þ
                        float targetYPos = itemVisualPos.y;
                        // �������� �� ��ġ
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
                            // itemvisual ������� ��ġ ���Ͻ� ��Ű��
                            GameObject newVisual = GameObject.Instantiate(itemVisual);
                            newVisual.transform.SetParent(GameObject.Find(EQUIP_VISUAL_PATH).transform);
                            equipSlot.itemVisual = newVisual;
                            RectTransform equipRectTrans = equipSlot.gameObject.GetComponent<RectTransform>();
                            RectTransform equipVisualRectTrans = newVisual.GetComponent<RectTransform>();
                            equipVisualRectTrans.anchorMin = new Vector2(0, 0);
                            equipVisualRectTrans.anchorMax = new Vector2(0, 0);
                            equipVisualRectTrans.anchoredPosition = equipRectTrans.anchoredPosition;
                            equipVisualRectTrans.sizeDelta = equipRectTrans.sizeDelta;
                            // �����ΰ�� UI ����ȭ
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
        // ����ִ� ������ �� �������� �ڵ�
        bool[,] emptyFlag = new bool[slotRowSize,slotColumnSize];
        int[] itemSize = GetItemSize(_item);
        List<Slot> targetedSlotList = new List<Slot>(); // Ž���� �̹� ������ ������ ����Ʈ
        Slot targetSlot = null; // Ÿ���� �� ����
        Vector2Int targetSlotIndex = new Vector2Int(0,0); // Ÿ�� ������ �ε��� ��ġ, y x
        Vector2 targetSlotPos = Vector2.zero;

        bool searchSuccessFlag = false; // �� ������ ã�� ���� Ž�� �÷���
        bool findTargetFlag; // Ÿ�� ������ ã���� true�� �Ǵ� �÷���

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
                Debug.Log("AllEmptySlot = (" + y + ", " + x + ")");
            }
        }
        // Ÿ�� ������ ã�������� �ݺ�
        while (searchSuccessFlag == false)
        {
            // Ž�� ���� ������ �ʱ�ȭ
            findTargetFlag = false;
            // ����ִ� ���� �ϳ��� ã�� �ڵ�
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
            bool re_searchFlag = false; // �� ������ �ٽ� Ž���ϴ� ���� ��Ÿ���� �÷���
            // Ž���ؾ��ϴ� ������(�������� ũ�Ⱑ) �ִ� ũ��(����ũ��)���� ū ��� ���� Ž������
            if (itemSize[0] + targetSlotIndex.x > slotRowSize || itemSize[1] + targetSlotIndex.y > slotColumnSize)
                continue;

            // ��� �ִ� ���� �ֺ��� �ٽ� Ž���ϴ� �ڵ�
            // Ž���� �����Ѵٸ� ���� Ž�� ����
            // Ž���� �����Ѵٸ� break�ؼ� �ٽ� �� �ڵ��
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
        // ������ ���� �ֱ�
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
                // �켱 �򰥸��� �ʰ� �׳� ĭ�� ���� �̹��� �ڴ°ɷ� �س���
                //if (nowSlot.transform.Find("Item/ItemImage").TryGetComponent<Image>(out Image _itemImage))
                //{
                //    _itemImage.color = new Color32(255, 255, 255, 255);
                //    _itemImage.sprite = Manager.Data.itemSprite[_item.itemImageNum];
                //}
                //else
                //    Debug.LogError("Can't Get Inventory Item Image Component");
            }
        }
        // ���� ���̴� ������ ������ ����
        GameObject itemVisual = GameObject.Instantiate(Manager.Data.itemUIPrefab[_item.itemIndex], GameObject.Find(INVENTORY_VISUAL_PATH).transform);
        itemVisual.transform.localPosition = InvenPosCal(itemVisual.transform.localPosition, targetSlotIndex);
        // itemVisual ���� ����
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
    public int[] GetItemSize(Item _item) // Enum�� �ִ� ���� ���� ������ ������ ������ ��ȯ�ϴ� �ڵ�, ������ �Ŵ����� ���߿� �ű�� �ҵ�
    {
        byte targetSize = (byte)_item.itemSize;
        byte ySize = (byte)(targetSize >> itemMaxSize); // �� 4��Ʈ Ȯ��
        byte xSize = (byte)(targetSize - (ySize << itemMaxSize)); // �� 4��Ʈ Ȯ��
        int[] convertedSize = {ySize,xSize};
        return convertedSize;
    }
}
