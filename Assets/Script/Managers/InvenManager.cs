using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

public class InvenManager
{
    public Dictionary<string, Slot> equipSlots = new Dictionary<string, Slot>();
    public List<SlotLine> invenSlotLines = new List<SlotLine>();
    public Interactive nowInteractive;

    private List<SlotLine> dropSlotLines = new List<SlotLine>();
    private List<SlotLine> stashSlotLines = new List<SlotLine>();

    #region itemBoxSize
    private int invenSlotRowSize = 5;
    private int invenSlotColumnSize = 9;

    private int stashSlotRowSize = 11;
    private int stashSlotColumnSize = 9;

    private int dropSlotRowSize = 5;
    private int dropSlotColumnSize = 9;
    #endregion

    #region itemSettingValue
    private int itemBitSize = 4; // 2^4 X 2^4 짜리가 최대 크기라고 가정
    private const int UNITSIZE = 80;
    private Vector2 standardPos = new Vector2(40, -40);
    #endregion

    #region stringPath
    private const string INVEN_CANVAS_PATH = "InvenCanvas";
    private const string STASH_CANVAS_PATH = "StashCanvas";
    private const string DROP_CANVAS_PATH = "DropCanvas";
    private const string INVENTORY_VISUAL_PATH = "InvenCanvas/Panel/ItemArea/ItemVisual";
    private const string EQUIP_UI_PATH = "GameUI/EquipUI";
    private const string ITEM_UI_TAG = "ItemUI";
    private const string INVENTORY_SLOT_TAG = "InvenSlot";
    private const string EQUIP_SLOT_TAG = "EquipSlot";
    private const string LOBBY_SCENE_NAME = "LobbyMerchantWork";
    #endregion

    #region variable
    private Canvas invenCanvas;
    private Canvas stashCanvas;
    private Canvas dropCanvas;
    private CanvasGroup invenCanvasGroup;
    private ItemInfo itemInfo;
    private bool canvasVisualFlag;
    private EquipArea equipArea;
    private EquipUI equipUI;
    #endregion

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
        if (Manager.Instance.GetNowScene().name == LOBBY_SCENE_NAME)
            return;
        // 게임 씬일 경우에만 작동하게 바꿔야함
        if (Input.GetKeyDown(KeyCode.Tab))
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
    private void DataAssign()
    {
        // Canvas 할당
        invenCanvas = GameObject.Find(INVEN_CANVAS_PATH).GetComponent<Canvas>();
        invenCanvasGroup = invenCanvas.GetComponent<CanvasGroup>();
        equipArea = invenCanvas.GetComponentInChildren<EquipArea>();

        stashCanvas = GameObject.Find(STASH_CANVAS_PATH).GetComponent<Canvas>();

        dropCanvas = GameObject.Find(DROP_CANVAS_PATH).GetComponent<Canvas>();

        // Canavs에 해당하는 Slot들 할당
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
        stashCanvas.gameObject.SetActive(false);
        dropCanvas.gameObject.SetActive(false);
    }
    public Vector2 InvenPosCal(Vector2 _originPos, Vector2 _slotIndex)
    {
        return _originPos + new Vector2 (_slotIndex.y, -_slotIndex.x) * UNITSIZE;
    }
    /// <summary>
    /// 랜덤한 아이템을 가진 리스트를 반환해주는 함수
    /// </summary>
    /// <param name="_minInclusiveItemNum">아이템 갯수의 최소 갯수</param>
    /// <param name="_maxExclusiveItemNum">아이템 갯수의 최대 갯수</param>
    /// <returns></returns>
    public List<Item> GetRandomItem(int _minInclusiveItemNum, int _maxExclusiveItemNum)
    {
        List<Item> randomItemList = new List<Item>();
        // 현재 생성될 갯수를 랜덤하게 정함
        int itemNum = UnityEngine.Random.Range(_minInclusiveItemNum, _maxExclusiveItemNum);
        // 아이템의 최대 갯수(최대 인덱스)
        int maxItemIndex = Manager.Data.itemData.Values.Count;
        List<Item> itemList = Manager.Data.itemData.Values.ToList();
        for (int i = 0; i < itemNum; i++)
        {
            // 생성할 아이템의 갯수만큼 생성될 아이템의 Index를 고름
            int targetIndex = UnityEngine.Random.Range(0, maxItemIndex);
            randomItemList.Add(itemList[targetIndex]);
        }
        return randomItemList;
    }
    public void ItemBoxReset(ItemBoxType _itemBoxType)
    {
        GameObject targetGameObject = null;
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
    // 고쳐야할 점, slot안에 itemImage가 있는 경우 Slot 안에서만 보임 << 다시보니 아닌거 같은데? 걍 크게 하면 문제 없을듯? << 따로 visual을 만들어서 해결
    public IEnumerator ItemManage()
    {
        float timer = 0f;
        while (true)
        {
            #region 마우스 커서
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
            if (Manager.Instance.GetNowScene().name != LOBBY_SCENE_NAME)
                equipUI = GameObject.Find(EQUIP_UI_PATH).GetComponent<EquipUI>();
            // 일단 마우스의 위치를 계속 탐색해서 정보 띄우는게 우선
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);
            Slot fromSlot = null;
            GameObject itemVisual = null;
            // 장비창 slot에 상호작용하는 경우 활성화되는 플래그
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
                        // 인벤토리 슬롯인 경우
                        if (go.gameObject.CompareTag(INVENTORY_SLOT_TAG) == true)
                            interactEquipFlag = false;
                        // 장비창(장착) 슬롯인 경우
                        if (go.gameObject.CompareTag(EQUIP_SLOT_TAG) == true)
                            interactEquipFlag = true;

                        fromSlot = _slot;
                    }
                }
            }
            // fromSlot 존재하지 않는다면 반복
            // 아이템 데이터가 존재하지 않는다면 반복
            if (fromSlot == null || fromSlot.emptyFlag == true || itemVisual == null)
            {
                timer = 0f;
                ConcealItemInfo();
                continue;
            }
            // SlotLine 찾기
            Canvas fromCanvas = fromSlot.transform.root.GetComponent<Canvas>();
            List<SlotLine> fromSlotLine = new List<SlotLine>();
            ItemBoxType fromItemBoxType = ItemBoxType.Null;
            if (fromCanvas.Equals(invenCanvas))
            {
                fromSlotLine = invenSlotLines;
                fromItemBoxType = ItemBoxType.Inventory;
            }
            else if(fromCanvas.Equals(stashCanvas))
            {
                fromSlotLine = stashSlotLines;
                fromItemBoxType = ItemBoxType.Stash;
            }
            else if(fromCanvas.Equals(stashCanvas))
            {
                fromSlotLine = dropSlotLines;
                fromItemBoxType = ItemBoxType.Drop;
            }
            Vector2Int fromPos = -Vector2Int.one;

            // 접근하려는 슬롯이 메인 슬롯이 아닐 경우, 메인슬롯의 아이템 데이터로 치환
            if (fromSlot.mainSlotFlag == false)
            {
                Vector2Int index = fromSlot.itemDataPos;
                fromSlot = fromSlotLine[index.x].mySlots[index.y];
            }

            for (int y = 0; y < invenSlotRowSize; y++)
            {
                if (invenSlotLines[y].mySlots.Contains(fromSlot))
                {
                    fromPos = new Vector2Int(y, invenSlotLines[y].mySlots.IndexOf(fromSlot));
                }
            }
            for (int y = 0; y < stashSlotRowSize; y++)
            {
                if (stashSlotLines.Count < 1)
                    break;
                if (stashSlotLines[y].mySlots.Contains(fromSlot))
                {
                    fromPos = new Vector2Int(y, stashSlotLines[y].mySlots.IndexOf(fromSlot));
                }
            }
            //Debug.Log(targetSlot.slotItem.itemName);
            timer += Time.deltaTime;
            if (timer > 0.5f)
            {
                RevealItemInfo(fromSlot.slotItem);
            }
            #endregion
            // 눌린 마우스 버튼키 받기
            int mousebutton = -1;
            // 좌클릭
            if (Input.GetMouseButtonDown(0))
                mousebutton = 0;
            // 우클릭
            else if (Input.GetMouseButtonDown(1))
                mousebutton = 1;

            // 클릭을 받은 경우
            if (mousebutton > -1)
            {
                // 정보 obj 숨기기
                timer = 0f;
                ConcealItemInfo();

                Transform itemVisualTrans = null;
                Vector2 itemVisualOriginPos = Vector2.zero;
                itemVisualTrans = itemVisual.transform;
                itemVisualOriginPos = itemVisualTrans.GetComponent<RectTransform>().anchoredPosition;
                int[] itemSize = GetItemSize(fromSlot.slotItem);
                if (itemSize == null)
                    continue;
                #region MouseButtonLeft
                while (mousebutton == 0)
                {
                    // 장비창에 있는 아이템은 드래그 불가능하게 우선 설정
                    if (interactEquipFlag == true)
                        break;
                    // 아이템 크기 때문에 배경이 보이게 설정
                    itemVisual.GetComponent<Image>().color = new Color32(255, 255, 255, 10);
                    // 마우스 위치에 따라 아이템의 위치가 이동하는 코드
                    // 문제점 : 위치가 마음대로 이동함 -> anchoredPosition과 ScreenPointToLocalPointInRectangle을 통해 해결함
                    Vector2 convertedMousePos;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle
                        (invenCanvas.transform as RectTransform, Input.mousePosition, invenCanvas.worldCamera, out convertedMousePos);
                    Vector2 offset = new Vector2((itemSize[1] - 1) * UNITSIZE / 2, -(itemSize[0] - 1) * UNITSIZE / 2);
                    itemVisualTrans.position = invenCanvas.transform.TransformPoint(convertedMousePos + offset);

                    // 마우스를 뗐을 때
                    if (Input.GetMouseButtonUp(0))
                    {
                        pointer = new PointerEventData(EventSystem.current);
                        pointer.position = Input.mousePosition;

                        raycastResults = new List<RaycastResult>();
                        EventSystem.current.RaycastAll(pointer, raycastResults);
                        Slot toSlot = null;
                        if (raycastResults.Count > 0)
                            foreach (var go in raycastResults)
                                if (go.gameObject.TryGetComponent(out Slot _slot))
                                    toSlot = _slot;
                        // 목표 지점에 toSlot이 존재하지 않는 경우
                        if (toSlot == null)
                        {
                            // 마우스가 인벤 밖인데 메인화면이면 아무것도 안함
                            if(pointer.position.x > 1050 || Manager.Instance.GetNowScene().name.ToString() == LOBBY_SCENE_NAME)
                            {
                                itemVisual.GetComponent<RectTransform>().anchoredPosition = itemVisualOriginPos;
                                itemVisual.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                                Debug.LogError("ToSlot Is Not Detected");
                                break;
                            }
                            // 마우스가 인벤 밖인데 게임화면이면 아이템을 버림
                            else
                            {
                                DumpItem(fromSlot.slotItem);
                                DeleteInvenItem(fromSlot, fromItemBoxType);
                                break;
                            }
                        }
                        List<SlotLine> toSlotLine = new List<SlotLine>();
                        Vector2Int toPos = -Vector2Int.one;
                        Vector2Int toStorageSize = -Vector2Int.one;
                        bool canMoveFlag = true;
                        // from, to slot 탐색
                        string toKey = null;
                        for (int y = 0; y < invenSlotRowSize; y++)
                        {
                            if (toPos != -Vector2Int.one)
                                break;
                            if (invenSlotLines[y].mySlots.Contains(toSlot))
                            {
                                toSlotLine = invenSlotLines;
                                toPos = new Vector2Int(y, invenSlotLines[y].mySlots.IndexOf(toSlot));
                                toStorageSize = new Vector2Int(invenSlotRowSize, invenSlotColumnSize);
                            }
                            if (equipSlots.Values.Contains(toSlot))
                                toKey = toSlot.gameObject.name;
                        }
                        for (int y = 0; y < stashSlotRowSize; y++)
                        {
                            if (toPos != -Vector2Int.one)
                                break;
                            if (stashSlotLines[y].mySlots.Contains(toSlot))
                            {
                                toSlotLine = stashSlotLines;
                                toPos = new Vector2Int(y, stashSlotLines[y].mySlots.IndexOf(toSlot));
                                toStorageSize = new Vector2Int(stashSlotRowSize, stashSlotColumnSize);
                            }
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
                                    if (toSlotLine[y].mySlots[x].emptyFlag == false)
                                    {
                                        canMoveFlag = false;
                                        break;
                                    }
                                }
                            }
                        }
                        // 드래그로 장착하려는 경우
                        if(toKey != null)
                        {
                            itemVisual.GetComponent<RectTransform>().anchoredPosition = itemVisualOriginPos;
                            itemVisual.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                            Debug.LogError("ToSlot Is EquipSlot. Can't Move");
                            break;
                        }
                        // 배경 투명화
                        itemVisual.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                        // 아이템이 들어갈 수 있는지 탐색
                        // 성공시 true 실패시 false를 반환
                        // toSlot은 마우스가 있는 위치의 칸을 주기 때문에 메인칸을 함수 안에서 찾아야함
                        // toKey는 장비창의 아이템일 경우에만 작동하기 때문에 null이 아닐 경우에는 장비 관련
                        // 드래그로 장착하는건 추후에 시간 남으면 할듯
                        
                        // 움직일 수 있다면 데이터를 이동
                        if (canMoveFlag == true)
                        {
                            for (int y = 0; y < itemSize[0]; y++)
                            {
                                for (int x = 0; x < itemSize[1]; x++)
                                {
                                    Slot nowFrom = fromSlotLine[fromPos.x + y].mySlots[fromPos.y + x];
                                    Slot nowTo = toSlotLine[toPos.x + y].mySlots[toPos.y + x];
                                    nowTo.SlotCopy(nowFrom, toPos);
                                    nowFrom.SlotReset();
                                }
                            }
                        }
                        // 움직일 수 없다면 에러
                        else
                        {
                            Debug.LogError("Can't Move Here");
                        }
                        if (canMoveFlag == false)
                        {
                            // 실패시 원래 위치로 itemVisual을 옮김
                            itemVisual.GetComponent<RectTransform>().anchoredPosition = itemVisualOriginPos;
                            break;
                        }
                        else
                        {
                            // 성공시, 장비창이 연관된 것이 아니라면 성공한 위치로 itemVisual을 옮김
                            itemVisual.transform.SetParent(GameObject.Find(toSlot.transform.root.name.ToString() + "Panel/ItemArea/ItemVisual").transform);
                            RectTransform visualRect = itemVisual.GetComponent<RectTransform>();
                            RectTransform toSlotRect = toSlot.GetComponent<RectTransform>();
                            visualRect.anchoredPosition = toSlotRect.anchoredPosition + new Vector2(0, -toPos.x * UNITSIZE)
                                + new Vector2((itemSize[1] - 1) * (UNITSIZE / 2), -(itemSize[0] - 1) * (UNITSIZE / 2));
                            break;
                        }
                    }
                    yield return null;
                }
                #endregion
                #region MouseButtonRight
                // 우클릭 장착 해제 관련
                if (mousebutton == 1)
                {
                    if (fromSlot == null)
                        continue;
                    if (itemVisual == null)
                        continue;
                    // 장비창에서 해제하는 경우
                    // targetSlot이 EquipSlot일 경우
                    // 장착 해제할 것이 무기일 경우
                    if(equipArea.weaponList.Contains(fromSlot))
                    {
                        if (equipUI != null)
                        {
                            for (int i = 0; i < equipArea.weaponList.Count; i++)
                            {
                                if (fromSlot.Equals(equipArea.weaponList[i]))
                                    equipUI.uiSlots[i].itemImage.sprite = null;
                            }
                        }
                    }
                    else if(equipArea.consumList.Contains(fromSlot))
                    {
                        if (equipUI != null)
                        {
                            for (int i = 0; i < equipArea.consumList.Count; i++)
                            {
                                if (fromSlot.Equals(equipArea.consumList[i]))
                                    equipUI.uiSlots[i + equipArea.weaponList.Count].itemImage.sprite = null;
                            }
                        }
                    }
                    if (interactEquipFlag == true)
                    {
                        // Global.PlayerArmorUnEquip(fromSlot);
                        // AddItem이 실패한경우 장착한 아이템을 바닥에 버림
                        bool addFlag = AddItem(fromSlot.slotItem, ItemBoxType.Inventory);
                        if (addFlag == false && Manager.Instance.GetNowScene().name.ToString() != LOBBY_SCENE_NAME)
                            DumpItem(fromSlot.slotItem);
                        else if (addFlag == false)
                            Debug.Log("Can't Unequip. Inven Is Full");
                        // AddItem이 성공한 경우는 그냥 장비창 리셋만 하면 됨
                        GameObject.Destroy(itemVisual);
                        fromSlot.SlotReset();
                        continue;
                    }
                    // 인벤토리창에서 장착하는 경우
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

                        // 마우스의 위치가 가운데에 위치한 경우 가까운 위치로 지정하기 위한 코드
                        float targetXPos = itemVisualPos.x;
                        // y 좌표는 양수가 아닌 음수로 늘어남
                        float targetYPos = itemVisualPos.y;
                        // 아이템이 들어갈 위치
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

                            // itemvisual 사이즈와 위치 동일시 시키기
                            GameObject newVisual = GameObject.Instantiate(itemVisual);
                            newVisual.transform.SetParent(equipArea.itemVisualTrans);
                            equipSlot.itemVisual = newVisual;
                            RectTransform equipRectTrans = equipSlot.gameObject.GetComponent<RectTransform>();

                            RectTransform equipVisualRectTrans = newVisual.GetComponent<RectTransform>();
                            equipVisualRectTrans.anchorMin = new Vector2(0, 0);
                            equipVisualRectTrans.anchorMax = new Vector2(1, 1);
                            equipVisualRectTrans.anchoredPosition = equipRectTrans.anchoredPosition;
                            equipVisualRectTrans.sizeDelta = equipRectTrans.sizeDelta;
                            // 무기나 소모품인경우 UI 동기화
                            if(Manager.Instance.GetNowScene().name != LOBBY_SCENE_NAME)
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
                            //Global.PlayerArmorEquip(fromSlot.slotItem);
                            Slot deleteSlot = fromSlotLine[originYIndex].mySlots[originXIndex];
                            DeleteInvenItem(deleteSlot, fromItemBoxType);
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
    public bool AddItem(Item _item, ItemBoxType _itemBoxType)
    {
        // 비어있는 슬롯을 다 가져오는 코드
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
        List<Slot> targetedSlotList = new List<Slot>(); // 탐색을 이미 진행한 슬롯의 리스트
        Slot targetSlot = null; // 타겟이 된 슬롯
        Vector2Int targetSlotIndex = new Vector2Int(0,0); // 타켓 슬롯의 인덱스 위치, y x

        bool searchSuccessFlag = false; // 빈 공간을 찾기 위한 탐색 플래그
        bool findTargetFlag; // 타겟 슬롯을 찾으면 true가 되는 플래그

        for(int y = 0; y < targetRowSize; y++)
        {
            for (int x = 0; x < targetColumnSize; x++)
            {
                Slot nowSlot = targetSlotLines[y].mySlots[x];
                if (nowSlot.emptyFlag == false) // 해당 슬롯이 꽉차있다면 패스
                    continue;
                emptyFlag[y, x] = true;
                //Debug.Log("AllEmptySlot = (" + y + ", " + x + ")");
            }
        }
        // 타겟 슬롯을 찾을때까지 반복
        while (searchSuccessFlag == false)
        {
            // 탐색 관련 변수들 초기화
            findTargetFlag = false;
            // 비어있는 슬롯 하나를 찾는 코드
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
            bool re_searchFlag = false; // 빈 슬롯을 다시 탐색하는 것을 나타내는 플래그
            // 탐색해야하는 슬롯이(아이템의 크기가) 최대 크기(가방크기)보다 큰 경우 다음 탐색으로
            if (itemSize[0] + targetSlotIndex.x > targetRowSize || itemSize[1] + targetSlotIndex.y > targetColumnSize)
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
        targetSlot.emptyFlag = false;
        targetSlot.mainSlotFlag = true;
        targetSlot.itemDataPos = targetSlotIndex;
        targetSlot.slotItem = _item.ItemDeepCopy();
        if(targetSlot.slotItem.randomStatFlag == false)
        {
            targetSlot.slotItem.ItemRandomStat();
            targetSlot.slotItem.randomStatFlag = true;
        }
        // 눈에 보이는 아이템 프리팹 생성
        GameObject itemVisual = GameObject.Instantiate(Manager.Data.itemUIPrefab[_item.itemIndex]);
        itemVisual.transform.SetParent(GameObject.Find(targetSlot.transform.root.name.ToString() + "Panel/ItemArea/ItemVisual").transform);
        RectTransform visualRect = itemVisual.GetComponent<RectTransform>();
        RectTransform targetSlotRect = targetSlot.GetComponent<RectTransform>();
        visualRect.anchoredPosition = targetSlotRect.anchoredPosition + new Vector2(0, -targetSlotIndex.x * UNITSIZE)
            + new Vector2((itemSize[1] - 1) * (UNITSIZE / 2), -(itemSize[0] - 1) * (UNITSIZE / 2));
        // itemVisual 정보 저장
        targetSlot.itemVisual = itemVisual;

        // 나머지 Slot 데이터도 세팅
        int minY = targetSlotIndex.x;
        int maxY = targetSlotIndex.x + itemSize[0];
        int minX = targetSlotIndex.y;
        int maxX = targetSlotIndex.y + itemSize[1];
        for (int y = minY; y < maxY; y++)
        {
            for (int x = minX; x < maxX; x++)
            {
                Slot nowSlot = invenSlotLines[y].mySlots[x];
                if (targetSlot.Equals(nowSlot))
                    nowSlot.mainSlotFlag = true;
                else
                    nowSlot.mainSlotFlag = false;
                nowSlot.emptyFlag = false;
                nowSlot.itemDataPos = targetSlotIndex;
            }
        }
        return true;
    }
    public void DeleteInvenItem(Slot _slot, ItemBoxType _itemBoxType)
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
    
    public void DumpItem(Item _item)
    {
        //Debug.Log("Item Dumped");
        GameObject dumpedItem3D = GameObject.Instantiate(Manager.Data.item3DPrefab[_item.itemIndex]);
        Item newItem = _item.ItemDeepCopy();
        dumpedItem3D.GetComponent<Item3D>().myItem = newItem;
        dumpedItem3D.transform.position = Manager.Game.Player.transform.position;
        dumpedItem3D.transform.position += new Vector3(0, 0.1f, 0);
    }
    public List<Item> GetInvenItems()
    {
        List<Item> items = new List<Item>();
        for (int y = 0; y < invenSlotRowSize; y++)
        {
            for(int x = 0; x < invenSlotColumnSize; x++)
            {
                if(invenSlotLines[y].mySlots[x].mainSlotFlag == true)
                {
                    items.Add(invenSlotLines[y].mySlots[x].slotItem);
                }
            }
        }
        return items;
    }
    public int[] GetItemSize(Item _item) // Enum에 있는 값을 실제 아이템 사이즈 값으로 반환하는 코드, 데이터 매니저로 나중에 옮기든 할듯
    {
        if(_item == null)
            return null;
        byte targetSize = (byte)_item.itemSize;
        byte ySize = (byte)(targetSize >> itemBitSize); // 앞 4비트 확인
        byte xSize = (byte)(targetSize - (ySize << itemBitSize)); // 뒤 4비트 확인
        int[] convertedSize = {ySize,xSize};
        return convertedSize;
    }
    public void RevealItemInfo(Item _item)
    {
        if (_item == null)
            return;
        itemInfo.gameObject.SetActive(true);
        Vector3 offset = new Vector3(-200, 200);
        RectTransform infoRect = itemInfo.gameObject.GetComponent<RectTransform>();
        infoRect.position = Input.mousePosition + offset;
        Vector3 zeroPoint = Camera.main.ViewportToScreenPoint(Vector3.zero);
        Vector3 onePoint = Camera.main.ViewportToScreenPoint(Vector3.one);
        infoRect.position = new Vector3(Mathf.Clamp(infoRect.position.x, zeroPoint.x - offset.x, onePoint.x + offset.x),
            Mathf.Clamp(infoRect.position.y, zeroPoint.y + offset.y, onePoint.y - offset.y));
        Stat itemStat = _item.itemStat;
        float stat1 = float.NaN;
        string stat1Name = null;
        float stat2 = itemStat.MoveSpeed;
        bool equipFlag = false;
        if(_item.itemType == ItemType.Equipment)
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
            itemInfo.itemPart.text = $"{_item.equipPart}";
        else
            itemInfo.itemPart.text = "";
        if(_item.itemType == ItemType.Equipment)
        {
            itemInfo.itemStat1.text = $"{stat1Name} : {stat1}";
            itemInfo.itemStat2.text = $"MoveSpeed : {stat2}";
        }
        else
        {
            itemInfo.itemStat1.text = "";
            itemInfo.itemStat2.text = "";
        }
        itemInfo.itemText.text = $"{_item.itemText}";
    }
    public void ConcealItemInfo()
    {
        itemInfo.gameObject.SetActive(false);
    }
    public void RevealInvenCanvasByBt()
    {
        canvasVisualFlag = true;
        invenCanvasGroup.alpha = 1f;
    }
    public void ConcealInvenCanvasByBt()
    {
        canvasVisualFlag = false;
        invenCanvasGroup.alpha = 0f;
    }
    public void RevealDropCanvas()
    {
        dropCanvas.gameObject.SetActive(true);
    }
    public void ConcealDropCanvas()
    {
        dropCanvas.gameObject.SetActive(false);
    }
}
