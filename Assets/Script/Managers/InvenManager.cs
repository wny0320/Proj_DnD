using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class InvenManager
{
    public Dictionary<string, Slot> equipSlots = new Dictionary<string, Slot>();
    public List<SlotLine> invenSlotLines = new List<SlotLine>();
    public Interactive nowInteractive;

    private List<SlotLine> dropSlotLines = new List<SlotLine>();
    public List<SlotLine> stashSlotLines = new List<SlotLine>();

    private List<SlotLine> recoverSlotLines = new List<SlotLine>();
    #region itemBoxSize
    private int invenSlotRowSize = 5;
    private int invenSlotColumnSize = 9;

    private int stashSlotRowSize = 11;
    private int stashSlotColumnSize = 9;

    private int dropSlotRowSize = 5;
    private int dropSlotColumnSize = 9;
    #endregion

    #region itemSettingValue
    private int itemBitSize = 4; // 2^4 X 2^4 ¥���� �ִ� ũ���� ����
    private const int UNITSIZE = 80;
    private Vector2 standardPos = new Vector2(40, -40);
    #endregion

    #region stringPath
    private const string INVEN_CANVAS_PATH = "InvenCanvas";
    private const string STASH_CANVAS_PATH = "StashCanvas";
    private const string DROP_CANVAS_PATH = "DropCanvas";
    private const string DRAG_CANVAS_PATH = "DragItemCanvas";
    private const string INVENTORY_VISUAL_PATH = "InvenCanvas/Panel/ItemArea/ItemVisual";
    private const string EQUIP_UI_PATH = "GameUI/EquipUI";
    private const string ITEM_UI_TAG = "ItemUI";
    private const string INVENTORY_SLOT_TAG = "InvenSlot";
    private const string EQUIP_SLOT_TAG = "EquipSlot";
    private const string TEMP_SLOT_PATH = "TempItemSlots";

    //������ʹ� �׽�Ʈ�� �����͵�
    private const string TEST_LOBBY_NAME = "LobbyMerchantWork";
    #endregion

    #region variable
    private Canvas invenCanvas;
    private Canvas stashCanvas;
    private Canvas dropCanvas;
    private Canvas dragItemCanvas;
    private CanvasGroup invenCanvasGroup;
    private CanvasGroup stashCanvasGroup;
    private CanvasGroup dropItemCanvasGroup;
    private ItemInfo itemInfo;
    private EquipArea equipArea;
    private EquipUI equipUI;
    private SlotLine tempSlotLine;

    public bool canvasVisualFlag;
    private bool dragItemFlag;
    #endregion

    public void OnGameSceneLoad(GameObject go)
    {
        equipUI = go.GetComponentInChildren<EquipUI>();

        for (int i = 0; i < 2; i++)
        {
            if (equipSlots[EquipPart.Weapon.ToString() + i].itemVisual != null)
                equipUI.uiSlots[i].itemImage.sprite = equipSlots[EquipPart.Weapon.ToString() + i]
                    .itemVisual.transform.GetChild(0).GetComponent<Image>().sprite;
            if (equipSlots[ItemType.Consumable.ToString() + i].itemVisual != null)
                equipUI.uiSlots[i+2].itemImage.sprite = equipSlots[ItemType.Consumable.ToString() + i]
                    .itemVisual.transform.GetChild(0).GetComponent<Image>().sprite;
        }
    }

    public void OnStart()
    {
        DataAssign();
    }
    public void OnUpdate()
    {
        InvenActive();
    }
    private void InvenActive()
    {
        if (invenCanvas == null)
            return;
        if (Manager.Instance.GetNowScene().name != SceneName.DungeonScene.ToString())
            return;
        if (Manager.Instance.sceneLoadFlag == true)
            return;
        if (Manager.Game.isPlayerAlive == false)
            return;
        if (dragItemFlag == true)
            return;
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (canvasVisualFlag == true)
            {
                canvasVisualFlag = false;
                invenCanvasGroup.alpha = 0f;
                invenCanvasGroup.blocksRaycasts = false;
                if (Manager.Instance.GetNowScene().name == SceneName.DungeonScene.ToString())
                    Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                canvasVisualFlag = true;
                invenCanvasGroup.alpha = 1f;
                invenCanvasGroup.blocksRaycasts = true;
                if (Manager.Instance.GetNowScene().name == SceneName.DungeonScene.ToString())
                    Cursor.lockState = CursorLockMode.None;
            }
        }
    }
    private void DataAssign()
    {
        // Canvas �Ҵ�
        invenCanvas = GameObject.Find(INVEN_CANVAS_PATH).GetComponent<Canvas>();
        invenCanvasGroup = invenCanvas.GetComponent<CanvasGroup>();
        equipArea = invenCanvas.GetComponentInChildren<EquipArea>();

        stashCanvas = GameObject.Find(STASH_CANVAS_PATH).GetComponent<Canvas>();
        stashCanvasGroup = stashCanvas.GetComponent<CanvasGroup>();

        dropCanvas = GameObject.Find(DROP_CANVAS_PATH).GetComponent<Canvas>();
        dropItemCanvasGroup = dropCanvas.GetComponent<CanvasGroup>();

        dragItemCanvas = GameObject.Find(DRAG_CANVAS_PATH).GetComponent<Canvas>();

        tempSlotLine = invenCanvas.transform.Find(TEMP_SLOT_PATH).GetComponent<SlotLine>();

        // Canavs�� �ش��ϴ� Slot�� �Ҵ�
        SlotLine[] invenLines = invenCanvas.GetComponentsInChildren<SlotLine>();
        foreach(SlotLine slotLine in invenLines)
            invenSlotLines.Add(slotLine);

        SlotLine[] stashLines = stashCanvas.GetComponentsInChildren<SlotLine>();
        foreach (SlotLine slotLine in stashLines)
            stashSlotLines.Add(slotLine);

        SlotLine[] dropLines = dropCanvas.GetComponentsInChildren<SlotLine>();
        foreach(SlotLine slotLine in dropLines)
            dropSlotLines.Add(slotLine);

        invenCanvasGroup.alpha = 0f;
        invenCanvasGroup.interactable = false;
        invenCanvasGroup.blocksRaycasts = false;

        stashCanvasGroup.alpha = 0f;
        stashCanvasGroup.interactable = false;
        stashCanvasGroup.blocksRaycasts = false;

        dropItemCanvasGroup.alpha = 0f;
        dropItemCanvasGroup.interactable = false;
        dropItemCanvasGroup.blocksRaycasts = false;

        canvasVisualFlag = false;
        string typeName = null;
        int count = 0;
        foreach (var type in Enum.GetValues(typeof(ItemType)))
        {
            typeName = type.ToString();
            if (typeName == ItemType.Consumable.ToString())
            {
                for (int i = 0; i < equipArea.consumList.Count; i++)
                {
                    equipSlots.Add(typeName + i, equipArea.consumList[i]);
                }
            }
            if (typeName == ItemType.Equipment.ToString())
            {
                foreach (var part in Enum.GetValues(typeof(EquipPart)))
                {
                    string partsName = part.ToString();
                    if (part.Equals(EquipPart.Weapon))
                    {
                        for (int i = 0; i < equipArea.weaponList.Count; i++)
                        {
                            equipSlots.Add(partsName + i, equipArea.weaponList[i]);
                        }
                    }
                    else
                    {
                        equipSlots.Add(partsName, equipArea.armorList[count]);
                        count++;
                    }
                }
            }
            else
                continue;
        }

        itemInfo = invenCanvas.GetComponentInChildren<ItemInfo>();
        itemInfo.gameObject.SetActive(false);
        MonoBehaviour.DontDestroyOnLoad(invenCanvas);
        MonoBehaviour.DontDestroyOnLoad(stashCanvas);
        MonoBehaviour.DontDestroyOnLoad(dropCanvas);
        MonoBehaviour.DontDestroyOnLoad(dragItemCanvas);
    }
    public Vector2 InvenPosCal(Vector2 _originPos, Vector2 _slotIndex)
    {
        return _originPos + new Vector2 (_slotIndex.y, -_slotIndex.x) * UNITSIZE;
    }
    /// <summary>
    /// ������ �������� ���� ����Ʈ�� ��ȯ���ִ� �Լ�
    /// </summary>
    /// <param name="_minInclusiveItemNum">������ ������ �ּ� ����</param>
    /// <param name="_maxExclusiveItemNum">������ ������ �ִ� ����</param>
    /// <returns></returns>
    public List<Item> GetRandomItem(int _minInclusiveItemNum, int _maxExclusiveItemNum)
    {
        List<Item> randomItemList = new List<Item>();
        // ���� ������ ������ �����ϰ� ����
        int itemNum = UnityEngine.Random.Range(_minInclusiveItemNum, _maxExclusiveItemNum);

        List<Item> itemList = Manager.Data.itemData.Values.ToList();

        List<Item> consumItemList = new List<Item>();
        List<Item> equipItemList = new List<Item>();
        List<Item> coinItemList = new List<Item>();
        List<Item> antiqueItemList = new List<Item>();

        // �������� Ÿ�Կ� ���� �з�
        foreach (var item in itemList)
        {
            if (item.itemType == ItemType.Consumable)
            {
                consumItemList.Add(item);
            }
            else if (item.itemType == ItemType.Equipment)
            {
                equipItemList.Add(item);
            }
            else if(item.itemType == ItemType.Coin)
            {
                coinItemList.Add(item);
            }
            else if(item.itemType == ItemType.Antique)
            {
                antiqueItemList.Add(item);
            }
        }
        for (int i = 0; i < itemNum; i++)
        {
            int dice = UnityEngine.Random.Range(0, 100);
            if(dice < 3) // 3% ��� coin Item
            {
                // ������ �������� ������ŭ ������ �������� Index�� ��
                int targetIndex = UnityEngine.Random.Range(0, coinItemList.Count);
                randomItemList.Add(coinItemList[targetIndex]);
            }
            else if(dice < 20) // 17% Consumable
            {
                // ������ �������� ������ŭ ������ �������� Index�� ��
                int targetIndex = UnityEngine.Random.Range(0, consumItemList.Count);
                randomItemList.Add(consumItemList[targetIndex]);
            }
            else if(dice < 50) // 30% Antique
            {
                // ������ �������� ������ŭ ������ �������� Index�� ��
                int targetIndex = UnityEngine.Random.Range(0, antiqueItemList.Count);
                randomItemList.Add(antiqueItemList[targetIndex]);
            }
            else
            {
                // ������ �������� ������ŭ ������ �������� Index�� ��
                int targetIndex = UnityEngine.Random.Range(0, equipItemList.Count);
                randomItemList.Add(equipItemList[targetIndex]);
            }
        }
        return randomItemList;
    }
    public void ItemBoxReset(ItemBoxType _itemBoxType)
    {
        int targetRowSize = 0;
        int targetColumnSize = 0;
        List<SlotLine> targetSlotLines = new List<SlotLine>();
        if (_itemBoxType == ItemBoxType.Inventory)
        {
            targetRowSize = invenSlotRowSize;
            targetColumnSize = invenSlotColumnSize;
            targetSlotLines = invenSlotLines;
        }
        if (_itemBoxType == ItemBoxType.Stash)
        {
            targetRowSize = stashSlotRowSize;
            targetColumnSize = stashSlotColumnSize;
            targetSlotLines = stashSlotLines;
        }
        if (_itemBoxType == ItemBoxType.Drop)
        {
            targetRowSize = dropSlotRowSize;
            targetColumnSize = dropSlotColumnSize;
            targetSlotLines = dropSlotLines;
        }
        for (int y = 0; y < targetRowSize; y++)
        {
            for (int x = 0; x < targetColumnSize; x++)
            {
                GameObject.Destroy(targetSlotLines[y].mySlots[x].itemVisual);
                targetSlotLines[y].mySlots[x].SlotReset();
            }
        }
    }
    // ���ľ��� ��, slot�ȿ� itemImage�� �ִ� ��� Slot �ȿ����� ���� << �ٽú��� �ƴѰ� ������? �� ũ�� �ϸ� ���� ������? << ���� visual�� ���� �ذ�
    public IEnumerator ItemManage()
    {
        float timer = 0f;
        while (true)
        {
            #region ���콺 Ŀ��
            yield return null;
            if(invenCanvas == null)
            {
                //Debug.LogError("Inventory Is Not Assigned");
                continue;
            }
            if(canvasVisualFlag == false)
            {
                //Debug.Log("Canvas Is Invisible");
                continue;
            }

            //if (equipUI == null && Manager.Instance.GetNowScene().name != SceneName.MainLobbyScene.ToString()
            //    && Manager.Instance.GetNowScene().name != TEST_LOBBY_NAME) // �׽�Ʈ��
            //    equipUI = GameObject.Find(EQUIP_UI_PATH).GetComponent<EquipUI>();

            // �ϴ� ���콺�� ��ġ�� ��� Ž���ؼ� ���� ���°� �켱
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);
            Slot fromSlot = null;
            GameObject itemVisual = null;
            // ���â slot�� ��ȣ�ۿ��ϴ� ��� Ȱ��ȭ�Ǵ� �÷���
            bool interactEquipFlag = false;
            if (raycastResults.Count > 0)
            {
                foreach (var go in raycastResults)
                {
                    if (go.gameObject.CompareTag(ITEM_UI_TAG) == true)
                    {
                        itemVisual = go.gameObject;
                        continue;
                    }
                    if (go.gameObject.TryGetComponent(out Slot _slot))
                    {
                        // �κ��丮 ������ ���
                        if (go.gameObject.CompareTag(INVENTORY_SLOT_TAG) == true)
                            interactEquipFlag = false;
                        // ���â(����) ������ ���
                        if (go.gameObject.CompareTag(EQUIP_SLOT_TAG) == true)
                            interactEquipFlag = true;

                        fromSlot = _slot;
                    }
                }
            }
            // fromSlot �������� �ʴ´ٸ� �ݺ�
            // ������ �����Ͱ� �������� �ʴ´ٸ� �ݺ�
            if (fromSlot == null || fromSlot.emptyFlag == true || itemVisual == null)
            {
                timer = 0f;
                ConcealItemInfo();
                continue;
            }
            // SlotLine ã��
            Canvas fromCanvas = fromSlot.transform.root.GetComponent<Canvas>();
            List<SlotLine> fromSlotLines = new List<SlotLine>();
            ItemBoxType fromItemBoxType = ItemBoxType.Null;
            Vector2Int fromStorageSize = -Vector2Int.one;
            if (fromCanvas.Equals(invenCanvas))
            {
                fromSlotLines = invenSlotLines;
                fromItemBoxType = ItemBoxType.Inventory;
                fromStorageSize = new Vector2Int(invenSlotRowSize, invenSlotColumnSize);
            }
            else if(fromCanvas.Equals(stashCanvas))
            {
                fromSlotLines = stashSlotLines;
                fromItemBoxType = ItemBoxType.Stash;
                fromStorageSize = new Vector2Int(stashSlotRowSize, stashSlotColumnSize);
            }
            else if(fromCanvas.Equals(dropCanvas))
            {
                fromSlotLines = dropSlotLines;
                fromItemBoxType = ItemBoxType.Drop;
                fromStorageSize = new Vector2Int(dropSlotRowSize, dropSlotColumnSize);
            }
            Vector2Int fromPos = -Vector2Int.one;
            // �����Ϸ��� ������ ���� ������ �ƴ� ���, ���ν����� ������ �����ͷ� ġȯ
            if (fromSlot.mainSlotFlag == false)
            {
                Vector2Int index = fromSlot.itemDataPos;
                fromSlot = fromSlotLines[index.x].mySlots[index.y];
            }
            for(int y = 0; y < fromStorageSize.x; y++)
            {
                if (fromSlotLines[y].mySlots.Contains(fromSlot))
                    fromPos = new Vector2Int(y, fromSlotLines[y].mySlots.IndexOf(fromSlot));
            }
            //Debug.Log(targetSlot.slotItem.itemName);
            timer += Time.deltaTime;
            if (timer > 0.5f)
            {
                RevealItemInfo(fromSlot.slotItem);
            }
            #endregion
            // ���� ���콺 ��ưŰ �ޱ�
            int mousebutton = -1;
            // ��Ŭ��
            if (Input.GetMouseButtonDown(0))
                mousebutton = 0;
            // ��Ŭ��
            else if (Input.GetMouseButtonDown(1))
                mousebutton = 1;

            // Ŭ���� ���� ���
            if (mousebutton > -1)
            {
                // ���� obj �����
                timer = 0f;
                ConcealItemInfo();

                Transform itemVisualTrans = null;
                Transform itemVisualParent = null;
                Vector2 itemVisualOriginPos = Vector2.zero;
                itemVisualTrans = itemVisual.transform;
                itemVisualParent = itemVisual.transform.parent;
                itemVisualOriginPos = itemVisualTrans.GetComponent<RectTransform>().anchoredPosition;
                int[] itemSize = GetItemSize(fromSlot.slotItem);
                if (itemSize == null)
                    continue;
                #region MouseButtonLeft
                while (mousebutton == 0)
                {
                    // ���â�� �ִ� �������� �巡�� �Ұ����ϰ� �켱 ����
                    if (interactEquipFlag == true)
                        break;
                    if (Manager.Game.isPlayerAlive == false)
                    {
                        GameObject.Destroy(itemVisual);
                        dragItemFlag = false;
                        for (int y = 0; y < itemSize[0]; y++)
                        {
                            for (int x = 0; x < itemSize[1]; x++)
                            {
                                tempSlotLine.mySlots[y * itemSize[1] + x].SlotReset();
                            }
                        }
                        break;
                    }
                    dragItemFlag = true;
                    // �巡���� ������ tempSlot���� �̵�
                    if (tempSlotLine.mySlots[0].slotItem == null)
                    {
                        for (int y = 0; y < itemSize[0]; y++)
                        {
                            for (int x = 0; x < itemSize[1]; x++)
                            {
                                Slot nowFromSlot = fromSlotLines[fromPos.x + y].mySlots[fromPos.y + x];
                                tempSlotLine.mySlots[y * itemSize[1] + x].SlotCopy(nowFromSlot, nowFromSlot.itemDataPos);
                                nowFromSlot.SlotReset();
                            }
                        }
                    }
                    // ������ ũ�� ������ ����� ���̰� ����
                    itemVisual.GetComponent<Image>().color = new Color32(255, 255, 255, 10);
                    // ���콺 ��ġ�� ���� �������� ��ġ�� �̵��ϴ� �ڵ�
                    itemVisualTrans.SetParent(dragItemCanvas.transform);
                    // ������ : ��ġ�� ������� �̵��� -> anchoredPosition�� ScreenPointToLocalPointInRectangle�� ���� �ذ���
                    Vector2 convertedMousePos;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle
                        (invenCanvas.transform as RectTransform, Input.mousePosition, invenCanvas.worldCamera, out convertedMousePos);
                    Vector2 offset = new Vector2((itemSize[1] - 1) * UNITSIZE / 2, -(itemSize[0] - 1) * UNITSIZE / 2);
                    itemVisualTrans.position = invenCanvas.transform.TransformPoint(convertedMousePos + offset);

                    // ���콺�� ���� ��
                    if (Input.GetMouseButtonUp(0))
                    {
                        dragItemFlag = false;
                        pointer = new PointerEventData(EventSystem.current);
                        pointer.position = Input.mousePosition;

                        raycastResults = new List<RaycastResult>();
                        EventSystem.current.RaycastAll(pointer, raycastResults);
                        Slot toSlot = null;
                        if (raycastResults.Count > 0)
                            foreach (var go in raycastResults)
                                if (go.gameObject.TryGetComponent(out Slot _slot))
                                    toSlot = _slot;
                        // ��ǥ ������ toSlot�� �������� �ʴ� ���
                        if (toSlot == null && fromCanvas.Equals(invenCanvas))
                        {
                            // ���콺�� �κ� ���ε� ����ȭ���̸� �ƹ��͵� ����
                            if (pointer.position.x > 1050 || Manager.Instance.GetNowScene().name.ToString() == SceneName.MainLobbyScene.ToString())
                            {
                                itemVisual.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                                itemVisual.transform.SetParent(GameObject.Find(fromSlot.transform.root.name.ToString() + "Panel/ItemArea/ItemVisual").transform);
                                itemVisual.GetComponent<RectTransform>().anchoredPosition = itemVisualOriginPos;
                                for (int y = 0; y < itemSize[0]; y++)
                                {
                                    for (int x = 0; x < itemSize[1]; x++)
                                    {
                                        Slot nowFrom = fromSlotLines[fromPos.x + y].mySlots[fromPos.y + x];
                                        nowFrom.SlotCopy(tempSlotLine.mySlots[y * itemSize[1] + x], fromPos);
                                        tempSlotLine.mySlots[y * itemSize[1] + x].SlotReset();
                                    }
                                }
                                Debug.LogError("ToSlot Is Not Detected");
                                break;
                            }
                            // ���콺�� �κ� ���ε� ����ȭ���̸� �������� ����
                            else
                            {
                                itemVisual.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                                itemVisual.transform.SetParent(GameObject.Find(fromSlot.transform.root.name.ToString() + "Panel/ItemArea/ItemVisual").transform);
                                itemVisual.GetComponent<RectTransform>().anchoredPosition = itemVisualOriginPos;
                                for (int y = 0; y < itemSize[0]; y++)
                                {
                                    for (int x = 0; x < itemSize[1]; x++)
                                    {
                                        Slot nowFrom = fromSlotLines[fromPos.x + y].mySlots[fromPos.y + x];
                                        nowFrom.SlotCopy(tempSlotLine.mySlots[y * itemSize[1] + x], fromPos);
                                        tempSlotLine.mySlots[y * itemSize[1] + x].SlotReset();
                                    }
                                }
                                DumpItem(fromSlot.slotItem);
                                DeleteBoxItem(fromSlot, fromItemBoxType);
                                break;
                            }
                        }
                        // invenCanvas�� fromSlot�� �ƴѵ� toSlot�� null�� ���
                        else if (toSlot == null)
                        {
                            GameObject.Destroy(itemVisual);
                            for (int y = 0; y < itemSize[0]; y++)
                            {
                                for (int x = 0; x < itemSize[1]; x++)
                                {
                                    Slot nowFrom = fromSlotLines[fromPos.x + y].mySlots[fromPos.y + x];
                                    nowFrom.SlotCopy(tempSlotLine.mySlots[y * itemSize[1] + x], fromPos);
                                    tempSlotLine.mySlots[y * itemSize[1] + x].SlotReset();
                                }
                            }
                            DumpItem(fromSlot.slotItem);
                            DeleteBoxItem(fromSlot, fromItemBoxType);
                            break;
                        }
                        List<SlotLine> toSlotLines = new List<SlotLine>();
                        Vector2Int toPos = -Vector2Int.one;
                        Vector2Int toStorageSize = -Vector2Int.one;
                        bool canMoveFlag = true;
                        // from, to slot Ž��
                        string toKey = null;
                        Canvas toCanvas = toSlot.transform.root.GetComponent<Canvas>();
                        if (toCanvas.Equals(invenCanvas))
                        {
                            toStorageSize = new Vector2Int(invenSlotRowSize, invenSlotColumnSize);
                            toSlotLines = invenSlotLines;
                        }
                        if(toCanvas.Equals(stashCanvas))
                        {
                            toStorageSize = new Vector2Int(stashSlotRowSize, stashSlotColumnSize);
                            toSlotLines = stashSlotLines;
                        }
                        if (toCanvas.Equals(dropCanvas))
                        {
                            toStorageSize = new Vector2Int(dropSlotRowSize, dropSlotColumnSize);
                            toSlotLines = dropSlotLines;
                        }
                        for (int y = 0; y < toStorageSize.x; y++)
                        {
                            if (toSlotLines[y].mySlots.Contains(toSlot))
                                toPos = new Vector2Int(y, toSlotLines[y].mySlots.IndexOf(toSlot));
                            if (equipSlots.Values.Contains(toSlot))
                                toKey = toSlot.name;
                        }
                        // �巡�׷� �����Ϸ��� ���, �κ��丮��� �ðܳ���
                        if(toKey != null && fromCanvas == invenCanvas)
                        {
                            itemVisual.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                            itemVisual.transform.SetParent(GameObject.Find(fromSlot.transform.root.name.ToString() + "Panel/ItemArea/ItemVisual").transform);
                            itemVisual.GetComponent<RectTransform>().anchoredPosition = itemVisualOriginPos;
                            for (int y = 0; y < itemSize[0]; y++)
                            {
                                for (int x = 0; x < itemSize[1]; x++)
                                {
                                    Slot nowFrom = fromSlotLines[fromPos.x + y].mySlots[fromPos.y + x];
                                    nowFrom.SlotCopy(tempSlotLine.mySlots[y * itemSize[1] + x], fromPos);
                                    tempSlotLine.mySlots[y * itemSize[1] + x].SlotReset();
                                }
                            }
                            Debug.LogError("ToSlot Is EquipSlot. Can't Move");
                            break;
                        }
                        else if(toKey != null && fromCanvas == dropCanvas)
                        {
                            GameObject.Destroy(itemVisual);
                            for (int y = 0; y < itemSize[0]; y++)
                            {
                                for (int x = 0; x < itemSize[1]; x++)
                                {
                                    Slot nowFrom = fromSlotLines[fromPos.x + y].mySlots[fromPos.y + x];
                                    nowFrom.SlotCopy(tempSlotLine.mySlots[y * itemSize[1] + x], fromPos);
                                    tempSlotLine.mySlots[y * itemSize[1] + x].SlotReset();
                                }
                            }
                            DumpItem(fromSlot.slotItem);
                            DeleteBoxItem(fromSlot, fromItemBoxType);
                            break;

                        }
                        if (toPos.x + itemSize[0] > toStorageSize.x || toPos.y + itemSize[1] > toStorageSize.y)
                            canMoveFlag = false;
                        if (canMoveFlag == true)
                        {
                            for (int y = toPos.x; y < toPos.x + itemSize[0]; y++)
                            {
                                if (canMoveFlag == false)
                                    break;
                                for (int x = toPos.y; x < toPos.y + itemSize[1]; x++)
                                {
                                    if (toSlotLines[y].mySlots[x].emptyFlag == false)
                                    {
                                        canMoveFlag = false;
                                        break;
                                    }
                                }
                            }
                        }
                        // ��� ����ȭ
                        itemVisual.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                        // �������� �� �� �ִ��� Ž��
                        // ������ true ���н� false�� ��ȯ
                        // toSlot�� ���콺�� �ִ� ��ġ�� ĭ�� �ֱ� ������ ����ĭ�� �Լ� �ȿ��� ã�ƾ���
                        // toKey�� ���â�� �������� ��쿡�� �۵��ϱ� ������ null�� �ƴ� ��쿡�� ��� ����
                        // �巡�׷� �����ϴ°� ���Ŀ� �ð� ������ �ҵ�
                        
                        // ������ �� �ִٸ� �����͸� �̵�
                        if (canMoveFlag == true)
                        {
                            for (int y = 0; y < itemSize[0]; y++)
                            {
                                for (int x = 0; x < itemSize[1]; x++)
                                {
                                    Slot nowTo = toSlotLines[toPos.x + y].mySlots[toPos.y + x];
                                    nowTo.SlotCopy(tempSlotLine.mySlots[y * itemSize[1] + x], toPos);
                                    tempSlotLine.mySlots[y * itemSize[1] + x].SlotReset();
                                }
                            }
                            // ������, ���â�� ������ ���� �ƴ϶�� ������ ��ġ�� itemVisual�� �ű�
                            itemVisual.transform.SetParent(GameObject.Find(toSlot.transform.root.name.ToString() + "Panel/ItemArea/ItemVisual").transform);
                            RectTransform visualRect = itemVisual.GetComponent<RectTransform>();
                            RectTransform toSlotRect = toSlot.GetComponent<RectTransform>();
                            visualRect.anchoredPosition = toSlotRect.anchoredPosition + new Vector2(0, -toPos.x * UNITSIZE)
                                + new Vector2((itemSize[1] - 1) * (UNITSIZE / 2), -(itemSize[0] - 1) * (UNITSIZE / 2));
                            break;
                        }
                        // ������ �� ���ٸ� ����
                        else
                        {
                            // ���н� ���� ��ġ�� itemVisual�� �ű�
                            Debug.LogError("Can't Move Here");
                            itemVisual.transform.SetParent(GameObject.Find(fromSlot.transform.root.name.ToString() + "Panel/ItemArea/ItemVisual").transform);
                            itemVisual.GetComponent<RectTransform>().anchoredPosition = itemVisualOriginPos;
                            for (int y = 0; y < itemSize[0]; y++)
                            {
                                for (int x = 0; x < itemSize[1]; x++)
                                {
                                    Slot nowFrom = fromSlotLines[fromPos.x + y].mySlots[fromPos.y + x];
                                    nowFrom.SlotCopy(tempSlotLine.mySlots[y * itemSize[1] + x], fromPos);
                                    tempSlotLine.mySlots[y * itemSize[1] + x].SlotReset();
                                }
                            }
                            break;
                        }
                    }
                    yield return null;
                }
                #endregion
                #region MouseButtonRight
                // ��Ŭ�� ���� ���� ����
                if (mousebutton == 1)
                {
                    if (fromSlot == null)
                        continue;
                    if (itemVisual == null)
                        continue;
                    // ���â���� �����ϰ� �������� �ƴҰ��
                    if(interactEquipFlag == true && !Manager.Game.isPlayerAttacking)
                    {
                        // AddItem�� �����Ѱ�� ������ �������� �ٴڿ� ����, �������� ���� ��� ���������� �����
                        Item newItem = AddItem(fromSlot.slotItem, ItemBoxType.Inventory);
                        if (newItem == null && Manager.Instance.GetNowScene().name.ToString() != SceneName.MainLobbyScene.ToString()
                            && Manager.Instance.GetNowScene().name.ToString() != TEST_LOBBY_NAME) // �׽�Ʈ��
                            DumpItem(fromSlot.slotItem);
                        else if (newItem == null)
                        {
                            Debug.Log("Can't Unequip. Inven Is Full");
                            continue;
                        }
                        // targetSlot�� EquipSlot�� ���
                        // ���� ������ ���� ������ ���
                        if (equipArea.weaponList.Contains(fromSlot))
                        {
                            if (equipUI != null)
                            {
                                for (int i = 0; i < equipArea.weaponList.Count; i++)
                                {
                                    if (fromSlot.Equals(equipArea.weaponList[i]))
                                    {
                                        equipUI.uiSlots[i].itemImage.sprite = null;
                                        if (i == Manager.Input.currentWeaponSlot)
                                            Global.PlayerWeaponEquip(null);
                                    }
                                }
                            }
                        }
                        // ���� ������ ���� �Ҹ�ǰ�� ���
                        else if (equipArea.consumList.Contains(fromSlot))
                        {
                            if (equipUI != null)
                            {
                                for (int i = 0; i < equipArea.consumList.Count; i++)
                                {
                                    if (fromSlot.Equals(equipArea.consumList[i]))
                                    {
                                        if (Manager.Game.isPlayerAttacking == false)
                                        {
                                            equipUI.uiSlots[i + equipArea.weaponList.Count].itemImage.sprite = null;
                                            if (i == Manager.Input.currentUtilitySlot)
                                                Global.PlayerWeaponEquip(null);
                                        }
                                    }
                                }
                            }
                        }
                        // ���� ������ ���� ���� ���
                        else
                            Global.PlayerArmorUnEquip(fromSlot);
                        // AddItem�� ������ ���� �׳� ���â ���¸� �ϸ� ��
                        GameObject.Destroy(itemVisual);
                        fromSlot.SlotReset();
                        continue;
                    }
                    // ���� �����Ϸ��� ������ �������� ���� �۵�X
                    else if(interactEquipFlag == true && Manager.Game.isPlayerAttacking)
                        continue;
                    // ������ �ڽ�â���� �����ϴ� ���
                    else
                    {
                        EquipPart targetEquipPart = fromSlot.slotItem.equipPart;
                        ItemType targetItemType = fromSlot.slotItem.itemType;
                        Slot equipSlot = null;
                        if(targetItemType == ItemType.Equipment)
                        {
                            if (targetEquipPart != EquipPart.Weapon)
                            {
                                if(equipSlots[targetEquipPart.ToString()].emptyFlag == true)
                                    equipSlot = equipSlots[targetEquipPart.ToString()];
                            }
                            if (targetEquipPart == EquipPart.Weapon)
                            {
                                foreach(Slot slot in equipArea.weaponList)
                                {
                                    if(slot.emptyFlag == true)
                                    {
                                        equipSlot = slot;
                                        break;
                                    }
                                }
                            }
                        }
                        else if(targetItemType == ItemType.Consumable)
                        {
                            foreach(Slot slot in equipArea.consumList)
                            {
                                if(slot.emptyFlag == true)
                                {
                                    equipSlot = slot;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError("Target Item Can't Equip");
                            continue;
                        }
                        Vector2 itemVisualPos = itemVisual.GetComponent<RectTransform>().anchoredPosition;

                        // ���콺�� ��ġ�� ����� ��ġ�� ��� ����� ��ġ�� �����ϱ� ���� �ڵ�
                        float targetXPos = itemVisualPos.x;
                        // y ��ǥ�� ����� �ƴ� ������ �þ
                        float targetYPos = itemVisualPos.y;
                        // �������� �� ��ġ
                        Vector2 targetPos = new Vector2(targetXPos, targetYPos);
                        int xIndex =
                            Mathf.RoundToInt(((targetXPos - (itemSize[1] - 1) * (UNITSIZE / 2)) - standardPos.x) / UNITSIZE);
                        int yIndex =
                            Mathf.RoundToInt(((targetYPos + (itemSize[0] - 1) * (UNITSIZE / 2)) - standardPos.y) / -UNITSIZE);
                        int originXIndex =
                            Mathf.RoundToInt(((itemVisualOriginPos.x - (itemSize[1] - 1) * (UNITSIZE / 2)) - standardPos.x) / UNITSIZE);
                        int originYIndex =
                            Mathf.RoundToInt(((Mathf.Abs(itemVisualOriginPos.y) - (itemSize[0] - 1) * (UNITSIZE / 2))
                            - Mathf.Abs(standardPos.y)) / UNITSIZE);
                        if (equipSlot != null)
                        {
                            equipSlot.slotItem = fromSlot.slotItem;
                            equipSlot.emptyFlag = false;
                            equipSlot.mainSlotFlag = true;

                            // itemvisual ������� ��ġ ���Ͻ� ��Ű��
                            GameObject newVisual = GameObject.Instantiate(itemVisual);
                            newVisual.transform.SetParent(equipArea.itemVisualTrans);
                            equipSlot.itemVisual = newVisual;
                            RectTransform equipRectTrans = equipSlot.GetComponent<RectTransform>();

                            RectTransform equipVisualRectTrans = newVisual.GetComponent<RectTransform>();
                            equipVisualRectTrans.anchorMin = new Vector2(0, 0);
                            equipVisualRectTrans.anchorMax = new Vector2(1, 1);
                            equipVisualRectTrans.anchoredPosition = equipRectTrans.anchoredPosition;
                            equipVisualRectTrans.sizeDelta = equipRectTrans.sizeDelta;
                            // ���⳪ �Ҹ�ǰ�ΰ�� UI ����ȭ
                            if(Manager.Instance.GetNowScene().name != SceneName.MainLobbyScene.ToString()
                                && Manager.Instance.GetNowScene().name != TEST_LOBBY_NAME)
                            {
                                if (targetItemType == ItemType.Equipment)
                                {
                                    if (targetEquipPart == EquipPart.Weapon)
                                    {
                                        int index = equipArea.weaponList.IndexOf(equipSlot);
                                        equipUI.uiSlots[index].itemImage.sprite = 
                                            itemVisual.transform.GetChild(0).GetComponent<Image>().sprite;
                                    }
                                }
                                else if (targetItemType == ItemType.Consumable)
                                {
                                    int index = equipArea.consumList.IndexOf(equipSlot);
                                    equipUI.uiSlots[index + equipArea.weaponList.Count].itemImage.sprite =
                                        itemVisual.transform.GetChild(0).GetComponent<Image>().sprite;
                                }
                            }
                            if (targetItemType == ItemType.Equipment)
                            {
                                if (targetEquipPart != EquipPart.Weapon)
                                    Global.PlayerArmorEquip(fromSlot.slotItem);
                            }
                            Slot deleteSlot = fromSlotLines[originYIndex].mySlots[originXIndex];
                            DeleteBoxItem(deleteSlot, fromItemBoxType);
                            //Debug.Log(GameObject.Find(targetImagePath).GetComponent<Image>().sprite);
                            continue;
                        }
                        else
                        {
                            Debug.LogError("Target Equip Slot Is Not Empty.");
                            continue;
                        }
                    }
                }

                #endregion
            }
        }
    }
    /// <summary>
    /// �ش��ϴ� Item�� �ش��ϴ� ItemBox�� �ش��ϴ� ItemRarity�� �����ϴ� �Լ�
    /// </summary>
    /// <param name="_item">�� �������� ���� Data</param>
    /// <param name="_itemBoxType">�������� �� ItemBox</param>
    /// <returns></returns>
    public Item AddItem(Item _item, ItemBoxType _itemBoxType, float _luck = 0f)
    {
        // ����ִ� ������ �� �������� �ڵ�
        bool[,] emptyFlag = null;
        int targetRowSize = 0;
        int targetColumnSize = 0;
        List<SlotLine> targetSlotLines = new List<SlotLine>();
        int[] itemSize = GetItemSize(_item);
        if (_itemBoxType == ItemBoxType.Inventory)
        {
            targetRowSize = invenSlotRowSize;
            targetColumnSize = invenSlotColumnSize;
            targetSlotLines = invenSlotLines;
        }
        if (_itemBoxType == ItemBoxType.Stash)
        {
            targetRowSize = stashSlotRowSize;
            targetColumnSize = stashSlotColumnSize;
            targetSlotLines = stashSlotLines;
        }
        if (_itemBoxType == ItemBoxType.Drop)
        {
            targetRowSize = dropSlotRowSize;
            targetColumnSize = dropSlotColumnSize;
            targetSlotLines = dropSlotLines;
        }
        emptyFlag = new bool[targetRowSize, targetColumnSize];
        List<Slot> targetedSlotList = new List<Slot>(); // Ž���� �̹� ������ ������ ����Ʈ
        Slot targetSlot = null; // Ÿ���� �� ����
        Vector2Int targetSlotIndex = new Vector2Int(0,0); // Ÿ�� ������ �ε��� ��ġ, y x

        bool searchSuccessFlag = false; // �� ������ ã�� ���� Ž�� �÷���
        bool findTargetFlag; // Ÿ�� ������ ã���� true�� �Ǵ� �÷���

        for(int y = 0; y < targetRowSize; y++)
        {
            for (int x = 0; x < targetColumnSize; x++)
            {
                Slot nowSlot = targetSlotLines[y].mySlots[x];
                if (nowSlot.emptyFlag == false) // �ش� ������ �����ִٸ� �н�
                    continue;
                emptyFlag[y, x] = true;
                //Debug.Log("AllEmptySlot = (" + y + ", " + x + ")");
            }
        }
        // Ÿ�� ������ ã�������� �ݺ�
        while (searchSuccessFlag == false)
        {
            // Ž�� ���� ������ �ʱ�ȭ
            findTargetFlag = false;
            // ����ִ� ���� �ϳ��� ã�� �ڵ�
            for (int y = 0;y < targetRowSize; y++)
            {
                if (findTargetFlag == true)
                    break;
                for(int x = 0;x < targetColumnSize; x++)
                {
                    if (emptyFlag[y, x] == true)
                    {
                        targetSlot = targetSlotLines[y].mySlots[x];
                        targetSlotIndex.x = y;
                        targetSlotIndex.y = x;
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
            if (itemSize[0] + targetSlotIndex.x > targetRowSize || itemSize[1] + targetSlotIndex.y > targetColumnSize)
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
            return null;
        }
        targetSlot.emptyFlag = false;
        targetSlot.mainSlotFlag = true;
        targetSlot.itemDataPos = targetSlotIndex;
        targetSlot.slotItem = _item.ItemDeepCopy(_luck);
        if (targetSlot.slotItem.equipStatSetFlag == false && targetSlot.slotItem.itemType == ItemType.Equipment)
        {
            targetSlot.slotItem.EquipStatSet();
            targetSlot.slotItem.equipStatSetFlag = true;
        }
        if(targetSlot.slotItem.equipStatSetFlag == false && targetSlot.slotItem.itemType == ItemType.Consumable)
        {
            targetSlot.slotItem.ConsumStatSet();
            targetSlot.slotItem.equipStatSetFlag = true;
        }
        // ���� ���̴� ������ ������ ����
        GameObject itemVisual = GameObject.Instantiate(Manager.Data.itemUIPrefab[_item.itemIndex]);
        itemVisual.transform.SetParent(GameObject.Find(targetSlot.transform.root.name.ToString() + "/Panel/ItemArea/ItemVisual").transform);
        RectTransform visualRect = itemVisual.GetComponent<RectTransform>();
        RectTransform targetSlotRect = targetSlot.GetComponent<RectTransform>();
        visualRect.anchoredPosition = targetSlotRect.anchoredPosition + new Vector2(0, -targetSlotIndex.x * UNITSIZE)
            + new Vector2((itemSize[1] - 1) * (UNITSIZE / 2), -(itemSize[0] - 1) * (UNITSIZE / 2));
        // itemVisual ���� ����
        targetSlot.itemVisual = itemVisual;
        Item targetItem = targetSlot.slotItem;

        // ������ Slot �����͵� ����
        int minY = targetSlotIndex.x;
        int maxY = targetSlotIndex.x + itemSize[0];
        int minX = targetSlotIndex.y;
        int maxX = targetSlotIndex.y + itemSize[1];
        for (int y = minY; y < maxY; y++)
        {
            for (int x = minX; x < maxX; x++)
            {
                Slot nowSlot = targetSlotLines[y].mySlots[x];
                if (targetSlot.Equals(nowSlot))
                    nowSlot.mainSlotFlag = true;
                else
                    nowSlot.mainSlotFlag = false;
                nowSlot.emptyFlag = false;
                nowSlot.itemDataPos = targetSlotIndex;
            }
        }
        Manager.Data.PlayerDataExport();
        return targetItem;
    }
    public void DeleteBoxItem(Slot _slot, ItemBoxType _itemBoxType)
    {
        List<SlotLine> targetSlotLines = new List<SlotLine>();
        if(_itemBoxType == ItemBoxType.Inventory)
        {
            targetSlotLines = invenSlotLines;
        }
        if(_itemBoxType == ItemBoxType.Stash)
        {
            targetSlotLines = stashSlotLines;
        }
        if(_itemBoxType == ItemBoxType.Drop)
        {
            targetSlotLines = dropSlotLines;
        }
        int[] itemSize = GetItemSize(_slot.slotItem);
        Vector2Int slotPos = _slot.itemDataPos;
        GameObject.Destroy(_slot.itemVisual);
        if(_itemBoxType == ItemBoxType.Equip)
        {
            if(equipArea.consumList.Contains(_slot))
            {
                int index = equipArea.consumList.IndexOf(_slot);
                equipUI.uiSlots[equipArea.weaponList.Count + index].itemImage.sprite = null;
            }
            _slot.SlotReset();
            return;
        }
        for(int y = 0;  y < itemSize[1]; y++)
        {
            for(int x = 0; x < itemSize[0]; x++)
            {
                Slot nowSlot = targetSlotLines[slotPos.x + x].mySlots[slotPos.y + y];
                nowSlot.SlotReset();
                //Debug.Log((_targetMainSlotIndex.x + x) + ", " + (_targetMainSlotIndex.y + y) + " Slot Delete Data");
            }
        }
    }
    public void ResetEquipSlots()
    {
        for(int i = 0; i < equipArea.weaponList.Count; i++)
        {
            GameObject.Destroy(equipArea.weaponList[i].itemVisual);
            equipArea.weaponList[i].SlotReset();
        }
        for (int i = 0; i < equipArea.consumList.Count; i++)
        {
            GameObject.Destroy(equipArea.consumList[i].itemVisual);
            equipArea.consumList[i].SlotReset();
        }
        for (int i = 0; i < equipArea.armorList.Count; i++)
        {
            GameObject.Destroy(equipArea.armorList[i].itemVisual);
            equipArea.armorList[i].SlotReset();
        }
    }
    public void DumpItem(Item _item)
    {
        GameObject dumpedItem3D = GameObject.Instantiate(Manager.Data.item3DPrefab[_item.itemIndex]);
        Item newItem = _item.ItemDeepCopy();
        dumpedItem3D.GetComponent<Item3D>().myItem = newItem;
        dumpedItem3D.transform.position = Manager.Game.Player.transform.position;
        dumpedItem3D.transform.position += new Vector3(0, 0.1f, 0);
    }
    public List<Item> GetBoxItems(ItemBoxType _itemBoxType)
    {
        Vector2Int storageSize = -Vector2Int.one;
        List<SlotLine> targetSlotLines = new List<SlotLine>();
        switch(_itemBoxType)
        {
            case ItemBoxType.Inventory:
                storageSize = new Vector2Int(invenSlotRowSize, invenSlotColumnSize);
                targetSlotLines = invenSlotLines;
                break;
            case ItemBoxType.Stash:
                storageSize = new Vector2Int(stashSlotRowSize, stashSlotColumnSize);
                targetSlotLines = stashSlotLines;
                break;
            case ItemBoxType.Drop:
                storageSize = new Vector2Int(dropSlotRowSize, dropSlotColumnSize);
                targetSlotLines = dropSlotLines;
                break;
            case ItemBoxType.Equip:
                break;
            case ItemBoxType.Null:
                break;
        }
        List<Item> items = new List<Item>();
        for (int y = 0; y < storageSize.x; y++)
        {
            for(int x = 0; x < storageSize.y; x++)
            {
                if(targetSlotLines[y].mySlots[x].mainSlotFlag == true)
                {
                    items.Add(targetSlotLines[y].mySlots[x].slotItem);
                }
            }
        }
        return items;
    }
    public int[] GetItemSize(Item _item) // Enum�� �ִ� ���� ���� ������ ������ ������ ��ȯ�ϴ� �ڵ�, ������ �Ŵ����� ���߿� �ű�� �ҵ�
    {
        if(_item == null)
            return null;
        byte targetSize = (byte)_item.itemSize;
        byte ySize = (byte)(targetSize >> itemBitSize); // �� 4��Ʈ Ȯ��
        byte xSize = (byte)(targetSize - (ySize << itemBitSize)); // �� 4��Ʈ Ȯ��
        int[] convertedSize = {ySize,xSize};
        return convertedSize;
    }
    public void RevealItemInfo(Item _item)
    {
        if (_item == null)
            return;
        itemInfo.gameObject.SetActive(true);
        RectTransform infoRect = itemInfo.gameObject.GetComponent<RectTransform>();
        RectTransform invenRect = invenCanvas.GetComponent<RectTransform>();
        Vector3 offset = new Vector3(-(invenRect.offsetMax.x - Mathf.Abs(infoRect.offsetMax.x) - Mathf.Abs(infoRect.offsetMin.x)),
            invenRect.offsetMax.y - Mathf.Abs(infoRect.offsetMax.y) - Mathf.Abs(infoRect.offsetMin.y)) / 2;
        infoRect.position = Input.mousePosition + offset;
        Vector3 zeroPoint = Camera.main.ViewportToScreenPoint(Vector3.zero);
        Vector3 onePoint = Camera.main.ViewportToScreenPoint(Vector3.one);
        infoRect.position = new Vector3(Mathf.Clamp(infoRect.position.x, zeroPoint.x - offset.x, onePoint.x + offset.x),
            Mathf.Clamp(infoRect.position.y, zeroPoint.y + offset.y, onePoint.y - offset.y));
        Stat itemStat = _item.itemStat;
        float stat1 = float.NaN;
        float stat2 = float.NaN;
        string stat1Name = null;
        if(itemStat != null)
            stat2 = itemStat.MoveSpeed;
        bool equipFlag = false;
        Image infoPanel = itemInfo.GetComponent<Image>();
        switch (_item.itemRarity)
        {
            case ItemRarity.Junk:
                infoPanel.color = new Color32(60, 60, 60, 200);
                break;
            case ItemRarity.Poor:
                infoPanel.color = new Color32(140, 140, 140, 200);
                break;
            case ItemRarity.Common:
                infoPanel.color = new Color32(70, 200, 70, 200);
                break;
            case ItemRarity.Rare:
                infoPanel.color = new Color32(60, 130, 200, 200);
                break;
            case ItemRarity.Epic:
                infoPanel.color = new Color32(120, 60, 200, 200);
                break;
            case ItemRarity.Legendary:
                infoPanel.color = new Color32(200, 40, 40, 200);
                break;
        }
        if (_item.itemType == ItemType.Equipment)
        {
            switch(_item.equipPart)
            {
                case EquipPart.Weapon:
                    stat1Name = "Attack";
                    stat1 = itemStat.Attack;
                    break;
                default:
                    stat1Name = "Defense";
                    stat1 = itemStat.Defense;
                    break;
            }
            equipFlag = true;
        }
        itemInfo.itemName.text = $"{_item.itemName}";
        itemInfo.itemType.text = $"{_item.itemType}";
        itemInfo.itemRarity.text = $"{_item.itemRarity}";
        if (equipFlag == true)
        {
            itemInfo.itemPart.text = $"{_item.equipPart}";
            itemInfo.itemStat1.text = $"{stat1Name} : {stat1}";
            itemInfo.itemStat2.text = $"MoveSpeed : {stat2}";
        }
        else
        {
            itemInfo.itemPart.text = "";
            itemInfo.itemStat1.text = $"";
            itemInfo.itemStat2.text = $"";
        }
        itemInfo.itemText.text = $"{_item.itemText}";
        itemInfo.itemPrice.text = $"ItemPrice : {_item.itemPrice}";
    }
    public void ConcealItemInfo()
    {
        itemInfo.gameObject.SetActive(false);
    }
    public void RevealInvenCanvasByBt()
    {
        canvasVisualFlag = true;
        invenCanvasGroup.alpha = 1f;
        invenCanvasGroup.blocksRaycasts = true;
    }
    public void ConcealInvenCanvasByBt()
    {
        canvasVisualFlag = false;
        invenCanvasGroup.alpha = 0f;
        invenCanvasGroup.blocksRaycasts = false;
    }
    public void RevealDropCanvas()
    {
        dropItemCanvasGroup.alpha = 1f;
        dropItemCanvasGroup.interactable = true;
        dropItemCanvasGroup.blocksRaycasts = true;
    }
    public void ConcealDropCanvas()
    {
        dropItemCanvasGroup.alpha = 0f;
        dropItemCanvasGroup.interactable = false;
        dropItemCanvasGroup.blocksRaycasts = false;
    }
    public void RevealStashCanvas()
    {
        stashCanvas.gameObject.SetActive(true);
        stashCanvasGroup.alpha = 1f;
        stashCanvasGroup.interactable = true;
    }
    public void ConcealStashCanvas()
    {
        stashCanvas.gameObject.SetActive(false);
    }
    public void RecoverPlayerItemData(JsonClass _slotJsonclass)
    {
        List<SlotLine> slotLines = invenSlotLines;

        int yCnt = _slotJsonclass.invenSlotLines.Count;
        for (int y = 0; y < yCnt; y++)
        {
            for(int x = 0; x < slotLines[y].mySlots.Count; x++)
            {
                Slot nowSlot = slotLines[y].mySlots[x];
                JsonSlot nowJsonSlot = _slotJsonclass.invenSlotLines[y].mySlots[x];
                nowSlot.JsonSlotToSlot(nowJsonSlot);
                // ���⼭ itemVisual ���� �۾��� �ؾ���
                if(nowSlot.mainSlotFlag == true)
                {
                    int[] itemSize = GetItemSize(nowSlot.slotItem);
                    GameObject itemVisual = GameObject.Instantiate(Manager.Data.itemUIPrefab[nowSlot.slotItem.itemIndex]);
                    itemVisual.transform.SetParent(GameObject.Find(nowSlot.transform.root.name.ToString() + "/Panel/ItemArea/ItemVisual").transform);
                    RectTransform visualRect = itemVisual.GetComponent<RectTransform>();
                    RectTransform targetSlotRect = nowSlot.GetComponent<RectTransform>();
                    visualRect.anchoredPosition = targetSlotRect.anchoredPosition + new Vector2(0, -nowSlot.itemDataPos.x * UNITSIZE)
                        + new Vector2((itemSize[1] - 1) * (UNITSIZE / 2), -(itemSize[0] - 1) * (UNITSIZE / 2));
                    // itemVisual ���� ����
                    nowSlot.itemVisual = itemVisual;
                }
            }
        }

        slotLines = stashSlotLines;

        yCnt = _slotJsonclass.stashSlotLines.Count;
        for (int y = 0; y < yCnt; y++)
        {
            for (int x = 0; x < slotLines[y].mySlots.Count; x++)
            {
                // ���⼭ itemVisual ���� �۾��� �ؾ���
                Slot nowSlot = slotLines[y].mySlots[x];
                JsonSlot nowJsonSlot = _slotJsonclass.stashSlotLines[y].mySlots[x];
                nowSlot.JsonSlotToSlot(nowJsonSlot);
                // ���⼭ itemVisual ���� �۾��� �ؾ���
                if (nowSlot.mainSlotFlag == true)
                {
                    int[] itemSize = GetItemSize(nowSlot.slotItem);
                    GameObject itemVisual = GameObject.Instantiate(Manager.Data.itemUIPrefab[nowSlot.slotItem.itemIndex], 
                        GameObject.Find(nowSlot.transform.root.name.ToString() + "/Panel/ItemArea/ItemVisual").transform);
                    RectTransform visualRect = itemVisual.GetComponent<RectTransform>();
                    visualRect.anchoredPosition = visualRect.anchoredPosition + new Vector2(nowSlot.itemDataPos.y * UNITSIZE, -nowSlot.itemDataPos.x * UNITSIZE);
                    // itemVisual ���� ����
                    nowSlot.itemVisual = itemVisual;
                }
            }
        }

        foreach(string key in _slotJsonclass.equipSlotDict.Keys)
        {
            Slot nowSlot = equipSlots[key];
            nowSlot.JsonSlotToSlot(_slotJsonclass.equipSlotDict[key]);

            JsonSlot nowJsonSlot = _slotJsonclass.equipSlotDict[key];

            nowSlot.JsonSlotToSlot(nowJsonSlot);
            if(nowJsonSlot.emptyFlag == false)
            {
                GameObject itemVisual = GameObject.Instantiate(Manager.Data.itemUIPrefab[nowSlot.slotItem.itemIndex]);
                itemVisual.transform.SetParent(equipArea.itemVisualTrans);
                nowSlot.itemVisual = itemVisual;
                RectTransform equipRectTrans = nowSlot.GetComponent<RectTransform>();

                RectTransform equipVisualRectTrans = itemVisual.GetComponent<RectTransform>();
                equipVisualRectTrans.anchorMin = new Vector2(0, 0);
                equipVisualRectTrans.anchorMax = new Vector2(1, 1);
                equipVisualRectTrans.anchoredPosition = equipRectTrans.anchoredPosition;
                equipVisualRectTrans.sizeDelta = equipRectTrans.sizeDelta;
            }
        }
    }
    public void MainEquipUIAlpha(int _index = -1)
    {
        int cnt = equipUI.uiSlots.Count;
        for(int i = 0; i < cnt; i++)
        {
            Image targetImage = equipUI.uiSlots[i].itemImage;
            if(_index == i)
                targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, 1f);
            else
                targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, 0.3f);
        }
    }
}
