using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;
using System.Linq;

public class InvenManager
{
    public Dictionary<string, Slot> equipSlots = new Dictionary<string, Slot>();
    private List<SlotLine> invenSlotLines = new List<SlotLine>();
    private List<SlotLine> stashSlotLines = new List<SlotLine>();
    private int invenSlotRowSize = 5;
    private int invenSlotColumnSize = 9;
    private int stashSlotRowSize = 11;
    private int stashSlotColumnSize = 9;
    private int storeSlotRowSize = 7;
    private int storeSlotColumnSize = 6;
    private int itemMaxSize = 4; // 2^4 X 2^4 짜리가 최대 크기라고 가정
    private const int UNITSIZE = 80;
    private Vector2 standardPos = new Vector2(40, -40);
    #region stringPath
    private const string INVENTORY_PATH = "InvenCanvas/Panel/ItemArea/Content";
    private const string EQUIP_PATH = "InvenCanvas/Panel/EquipArea/Slots";
    private const string EQUIP_VISUAL_PATH = "InvenCanvas/Panel/EquipArea/ItemVisual";
    private const string INVENTORY_VISUAL_PATH = "InvenCanvas/Panel/ItemArea/ItemVisual";
    private const string EQUIP_UI_PATH = "GameUI/EquipUI/Slot";
    private const string ITEM_UI_TAG = "ItemUI";
    private const string INVENTORY_SLOT_TAG = "InvenSlot";
    private const string EQUIP_SLOT_TAG = "EquipSlot";
    private const string ITEMINFO_PATH = "InvenCanvas/ItemInfoPanel";
    private string[] itemInfoNames = { "ItemName", "ItemType", "ItemRarity", "ItemPart", "ItemStats/ItemStat1", "ItemStats/ItemStat2", "ItemText" };
    #endregion
    private List<TMP_Text> itemInfoTexts;
    private Canvas invenCanvas;
    private Canvas stashCanvas;
    private CanvasGroup invenCanvasGroup;
    private GameObject itemInfo;
    private bool canvasVisualFlag;

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
        Transform invenTrans = GameObject.Find(INVENTORY_PATH).transform;
        for (int y = 0; y < invenSlotRowSize; y++)
        {
            if (invenTrans.Find("SlotLine" + y).TryGetComponent(out SlotLine _line))
                invenSlotLines.Add(_line);
            else
                Debug.LogError("SlotLine Is Not Assigned");
            for (int x = 0; x < invenSlotColumnSize; x++)
            {
                if (invenSlotLines[y] == null)
                    Debug.LogError("Line Is Not Assigned");
                if (invenTrans.Find("SlotLine" + y + "/Slot" + x).TryGetComponent(out Slot _slot))
                    invenSlotLines[y].mySlots.Add(_slot);
            }
        }
        invenCanvas = GameObject.Find("InvenCanvas").GetComponent<Canvas>();
        invenCanvasGroup = invenCanvas.GetComponent<CanvasGroup>();
        invenCanvasGroup.alpha = 0f;
        invenCanvasGroup.interactable = false;
        canvasVisualFlag = false;

        string typeName = null;
        foreach (var type in Enum.GetValues(typeof(ItemType)))
        {
            typeName = type.ToString();
            if (typeName == ItemType.Consumable.ToString())
            {
                equipSlots.Add(typeName + 1, GameObject.Find(EQUIP_PATH + "/" + typeName + 1).GetComponent<Slot>());
                equipSlots.Add(typeName + 2, GameObject.Find(EQUIP_PATH + "/" + typeName + 2).GetComponent<Slot>());
            }
            if (typeName == ItemType.Equipment.ToString())
            {
                foreach (var part in Enum.GetValues(typeof(EquipPart)))
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
            else
                continue;
        }

        itemInfo = GameObject.Find(ITEMINFO_PATH);
        itemInfoTexts = new List<TMP_Text>();
        foreach (var txt in itemInfoNames)
        {
            itemInfoTexts.Add(GameObject.Find(ITEMINFO_PATH + "/" + txt).GetComponent<TMP_Text>());
        }
        itemInfo.SetActive(false);
        MonoBehaviour.DontDestroyOnLoad(invenCanvas);
    }
    private void StatshDataAsggin()
    {
        SlotLine[] lines = stashCanvas.GetComponentsInChildren<SlotLine>();
        for(int y = 0; y < stashSlotRowSize; y++)
        {
            stashSlotLines.Add(lines[y]);
            for(int x = 0; x < stashSlotColumnSize; x++)
            {
                stashSlotLines[y].mySlots.Add(lines[y].mySlots[x]);
            }
        }
        MonoBehaviour.DontDestroyOnLoad(stashCanvas);
    }
    public Vector2 InvenPosCal(Vector2 _originPos, Vector2 _slotIndex)
    {
        return _originPos + new Vector2 (_slotIndex.y, -_slotIndex.x) * UNITSIZE;
    }
    // 고쳐야할 점, slot안에 itemImage가 있는 경우 Slot 안에서만 보임 << 다시보니 아닌거 같은데? 걍 크게 하면 문제 없을듯? << 따로 visual을 만들어서 해결
    public IEnumerator ItemManage()
    {
        float timer = 0f;
        while (true)
        {
            yield return null;
            if(invenCanvas == null)
            {
                Debug.LogError("Inventory Is Not Assigned");
                continue;
            }
            if(canvasVisualFlag == false)
            {
                Debug.Log("Canvas Is Invisible");
                continue;
            }
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
            // 접근한 Slot의 부모 Canvas를 받아오는 코드
            fromSlot.transform.root.TryGetComponent<Canvas>(out Canvas fromCanvas);
            if (stashCanvas == null && fromCanvas.gameObject.name == "StashCanvas")
            {
                stashCanvas = fromCanvas;
                StatshDataAsggin();
            }
            // SlotLine 찾기
            List<SlotLine> fromSlotLine = new List<SlotLine>();
            Vector2Int fromPos = -Vector2Int.one;
            for (int y = 0; y < invenSlotRowSize; y++)
            {
                if (invenSlotLines[y].mySlots.Contains(fromSlot))
                {
                    fromSlotLine = invenSlotLines;
                }
            }
            for (int y = 0; y < stashSlotRowSize; y++)
            {
                if (stashSlotLines.Count < 1)
                    break;
                if (stashSlotLines[y].mySlots.Contains(fromSlot))
                {
                    fromSlotLine = stashSlotLines;
                }
            }
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
                            if(pointer.position.x > 1050 || Manager.Instance.GetNowScene().name.ToString() == "LobbyMerchantWork")
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
                                DeleteInvenItem(fromSlot, fromSlotLine);
                                break;
                            }
                        }
                        toSlot.transform.root.TryGetComponent<Canvas>(out Canvas toCanvas);
                        // 위에서도 캔버스 업데이트가 안되었을 경우를 대비한 코드
                        if(stashCanvas == null && toCanvas.gameObject.name == "StashCanvas")
                        {
                            stashCanvas = toCanvas;
                            StatshDataAsggin();
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
                            for (int y = toPos.x; y < toPos.x + itemSize[0] - 1; y++)
                            {
                                for (int x = toPos.y; x < toPos.y + itemSize[1] - 1; x++)
                                {
                                    if (toSlotLine[y].mySlots[x].emptyFlag == false)
                                    {
                                        canMoveFlag = false;
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
                    string typeName = null;
                    foreach (var type in Enum.GetValues(typeof(ItemType)))
                    {
                        string targetItemTypeName = fromSlot.slotItem.itemType.ToString();
                        if (targetItemTypeName == ItemType.Consumable.ToString())
                            typeName = ItemType.Consumable.ToString();
                        if (targetItemTypeName == ItemType.Equipment.ToString())
                            typeName = ItemType.Equipment.ToString();
                        if (targetItemTypeName == ItemType.Coin.ToString())
                            typeName = ItemType.Coin.ToString();
                        if (targetItemTypeName == ItemType.Antique.ToString())
                            typeName = ItemType.Antique.ToString();
                    }
                    // 장비창에서 해제하는 경우
                    // targetSlot이 EquipSlot일 경우
                    // 장착 해제할 것이 장비일 경우
                    if (interactEquipFlag == true && typeName == ItemType.Equipment.ToString())
                    {
                        foreach(var part in Enum.GetValues(typeof(EquipPart)))
                        {
                            // 1번칸 무기인경우
                            string targetImagePath;
                            if (part.ToString() + 1 == fromSlot.gameObject.name.ToString())
                                targetImagePath = EQUIP_UI_PATH + 1 + "/ItemImage";
                            // 2번칸 무기인경우
                            else if (part.ToString() + 2 == fromSlot.gameObject.name.ToString())
                                targetImagePath = EQUIP_UI_PATH + 2 + "/ItemImage";
                            else
                                targetImagePath = null;
                            if(targetImagePath != null)
                                GameObject.Find(targetImagePath).GetComponent<Image>().sprite = null;
                            //Debug.Log(targetImagePath);
                        }
                        Global.PlayerArmorUnEquip(fromSlot);
                        // AddItem이 실패한경우 장착한 아이템을 바닥에 버림
                        bool addFlag = AddItem(fromSlot.slotItem);
                        if (addFlag == false && Manager.Instance.GetNowScene().name.ToString() != "LobbyMerchantWork")
                            DumpItem(fromSlot.slotItem);
                        else if (addFlag == false)
                            Debug.Log("Can't Unequip. Inven Is Full");
                        // AddItem이 성공한 경우는 그냥 장비창 리셋만 하면 됨
                        GameObject.Destroy(itemVisual);
                        fromSlot.SlotReset();
                        continue;
                    }
                    // 장비창에서 해제하는 경우
                    // 장착 해제할 것이 퀵슬롯템(소모품)일 경우
                    else if(interactEquipFlag == true && typeName == ItemType.Consumable.ToString())
                    {
                        // 1번칸 소모품인 경우
                        string targetImagePath;
                        if (typeName + 1 == fromSlot.gameObject.name.ToString())
                            targetImagePath = EQUIP_UI_PATH + 3 + "/ItemImage";
                        // 2번칸 소모품인 경우
                        else if (typeName + 2 == fromSlot.gameObject.name.ToString())
                            targetImagePath = EQUIP_UI_PATH + 4 + "/ItemImage";
                        else
                            targetImagePath = null;
                        if (targetImagePath != null)
                            GameObject.Find(targetImagePath).GetComponent<Image>().sprite = null;
                        bool addFlag = AddItem(fromSlot.slotItem);
                        if (addFlag == false && Manager.Instance.GetNowScene().name.ToString() != "LobbyMerchantWork")
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
                        string equipPartsName = null;
                        if(targetItemType == ItemType.Equipment)
                        {
                            if (targetEquipPart != EquipPart.Weapon)
                            {
                                equipPartsName = targetEquipPart.ToString();
                                equipSlot = equipSlots[equipPartsName];
                            }
                            if (targetEquipPart == EquipPart.Weapon)
                            {
                                string weaponPartsName = targetEquipPart.ToString();
                                Slot weaponSlot1 = equipSlots[weaponPartsName + 1];
                                Slot weaponSlot2 = equipSlots[weaponPartsName + 2];
                                // 무기 1슬롯이 빈 상황
                                if (weaponSlot1.emptyFlag == true)
                                {
                                    equipSlot = weaponSlot1;
                                    equipPartsName = weaponPartsName + 1;
                                }
                                // 그 외는 2번 슬롯, 2번 슬롯이 어차피 비었는지 안비었는지는 아래에서 또 확인할거라 패스함
                                else
                                {
                                    equipSlot = weaponSlot2;
                                    equipPartsName = weaponPartsName + 2;
                                }
                            }
                        }
                        else if(targetItemType == ItemType.Consumable)
                        {
                            string consumableName = targetItemType.ToString();
                            Slot consumableSlot1 = equipSlots[targetItemType.ToString() + 1];
                            Slot consumableSlot2 = equipSlots[targetItemType.ToString() + 2];
                            // 소비 1슬롯이 빈 상황
                            if (consumableSlot1.emptyFlag == true)
                            {
                                equipSlot = consumableSlot1;
                                equipPartsName = consumableName + 1;
                            }
                            // 그 외는 2번 슬롯, 2번 슬롯이 어차피 비었는지 안비었는지는 아래에서 또 확인할거라 패스함
                            else
                            {
                                equipSlot = consumableSlot2;
                                equipPartsName = consumableName + 2;
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
                        if (equipSlot.emptyFlag == true)
                        {
                            equipSlot.slotItem = fromSlot.slotItem;
                            equipSlot.emptyFlag = false;
                            equipSlot.mainSlotFlag = true;
                            // itemvisual 사이즈와 위치 동일시 시키기
                            GameObject newVisual = GameObject.Instantiate(itemVisual);
                            newVisual.transform.SetParent(GameObject.Find(EQUIP_VISUAL_PATH).transform);
                            equipSlot.itemVisual = newVisual;
                            RectTransform equipRectTrans = equipSlot.gameObject.GetComponent<RectTransform>();
                            RectTransform equipVisualRectTrans = newVisual.GetComponent<RectTransform>();
                            equipVisualRectTrans.anchorMin = new Vector2(0, 0);
                            equipVisualRectTrans.anchorMax = new Vector2(1, 1);
                            equipVisualRectTrans.anchoredPosition = equipRectTrans.anchoredPosition;
                            equipVisualRectTrans.sizeDelta = equipRectTrans.sizeDelta;
                            // 무기나 소모품인경우 UI 동기화
                            string targetImagePath = null;
                            if(targetItemType == ItemType.Equipment)
                            {
                                if (targetEquipPart == EquipPart.Weapon)
                                {
                                    if (equipPartsName == targetEquipPart.ToString() + 1)
                                        targetImagePath = EQUIP_UI_PATH + 1 + "/ItemImage";
                                    else
                                        targetImagePath = EQUIP_UI_PATH + 2 + "/ItemImage";
                                    GameObject.Find(targetImagePath).GetComponent<Image>().sprite =
                                        itemVisual.transform.GetChild(0).GetComponent<Image>().sprite;
                                }
                            }
                            else if(targetItemType == ItemType.Consumable)
                            {
                                if (equipPartsName == targetItemType.ToString() + 1)
                                    targetImagePath = EQUIP_UI_PATH + 3 + "/ItemImage";
                                else
                                    targetImagePath = EQUIP_UI_PATH + 4 + "/ItemImage";
                                GameObject.Find(targetImagePath).GetComponent<Image>().sprite =
                                        itemVisual.transform.GetChild(0).GetComponent<Image>().sprite;
                            }
                            Global.PlayerArmorEquip(fromSlot.slotItem);
                            Slot deleteSlot = fromSlotLine[originYIndex].mySlots[originXIndex];
                            DeleteInvenItem(deleteSlot, fromSlotLine);
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
    public bool AddItem(Item _item)
    {
        // 비어있는 슬롯을 다 가져오는 코드
        bool[,] emptyFlag = new bool[invenSlotRowSize,invenSlotColumnSize];
        int[] itemSize = GetItemSize(_item);
        List<Slot> targetedSlotList = new List<Slot>(); // 탐색을 이미 진행한 슬롯의 리스트
        Slot targetSlot = null; // 타겟이 된 슬롯
        Vector2Int targetSlotIndex = new Vector2Int(0,0); // 타켓 슬롯의 인덱스 위치, y x
        Vector2 targetSlotPos = Vector2.zero;

        bool searchSuccessFlag = false; // 빈 공간을 찾기 위한 탐색 플래그
        bool findTargetFlag; // 타겟 슬롯을 찾으면 true가 되는 플래그

        for(int y = 0; y < invenSlotRowSize; y++)
        {
            for (int x = 0; x < invenSlotColumnSize; x++)
            {
                Slot nowSlot = invenSlotLines[y].mySlots[x];
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
            for (int y = 0;y < invenSlotRowSize;y++)
            {
                if (findTargetFlag == true)
                    break;
                for(int x = 0;x < invenSlotColumnSize;x++)
                {
                    if (emptyFlag[y, x] == true)
                    {
                        targetSlot = invenSlotLines[y].mySlots[x];
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
            if (itemSize[0] + targetSlotIndex.x > invenSlotRowSize || itemSize[1] + targetSlotIndex.y > invenSlotColumnSize)
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
        //Debug.Log("TargetSlotPos = " + targetSlotIndex);
        targetSlot.emptyFlag = false;
        targetSlot.mainSlotFlag = true;
        targetSlot.itemDataPos = targetSlotIndex;
        targetSlot.slotItem = _item.ItemDeepCopy();
        if(targetSlot.slotItem.randomStatFlag == false)
        {
            targetSlot.slotItem.ItemRandomStat();
            targetSlot.slotItem.randomStatFlag = true;
        }
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
        // 눈에 보이는 아이템 프리팹 생성
        GameObject itemVisual = GameObject.Instantiate(Manager.Data.itemUIPrefab[_item.itemIndex], GameObject.Find(INVENTORY_VISUAL_PATH).transform);
        itemVisual.transform.localPosition = InvenPosCal(itemVisual.transform.localPosition, targetSlotIndex);
        // itemVisual 정보 저장
        targetSlot.itemVisual = itemVisual;
        return true;
    }
    public void DeleteInvenItem(Slot _slot, List<SlotLine> _targetSlotLine = null)
    {
        int[] itemSize = GetItemSize(_slot.slotItem);
        Vector2Int slotPos = _slot.itemDataPos;
        GameObject.Destroy(_slot.itemVisual);
        if(_targetSlotLine == null)
        {
            //임시 코드일 수도?
            int _path;
            if (_slot.name[_slot.name.Length - 1] == '1') _path = 3;
            else _path = 4;
            GameObject.Find(EQUIP_UI_PATH + _path + "/ItemImage").GetComponent<Image>().sprite = null;
            //여기까지
            _slot.SlotReset();
            return;
        }
        for(int y = 0;  y < itemSize[1]; y++)
        {
            for(int x = 0; x < itemSize[0]; x++)
            {
                Slot nowSlot = _targetSlotLine[slotPos.x + x].mySlots[slotPos.y + y];
                nowSlot.SlotReset();
                //Debug.Log((_targetMainSlotIndex.x + x) + ", " + (_targetMainSlotIndex.y + y) + " Slot Delete Data");
            }
        }
    }
    private void DumpItem(Item _item)
    {
        //Debug.Log("Item Dumped");
        GameObject dumpedItem3D = GameObject.Instantiate(Manager.Data.item3DPrefab[_item.itemIndex]);
        Item newItem = _item.ItemDeepCopy();
        dumpedItem3D.GetComponent<Item3D>().myItem = newItem;
        dumpedItem3D.transform.position = Manager.Game.Player.transform.position;
        dumpedItem3D.transform.position += new Vector3(0, 0.1f, 0);
    }
    public int[] GetItemSize(Item _item) // Enum에 있는 값을 실제 아이템 사이즈 값으로 반환하는 코드, 데이터 매니저로 나중에 옮기든 할듯
    {
        if(_item == null)
            return null;
        byte targetSize = (byte)_item.itemSize;
        byte ySize = (byte)(targetSize >> itemMaxSize); // 앞 4비트 확인
        byte xSize = (byte)(targetSize - (ySize << itemMaxSize)); // 뒤 4비트 확인
        int[] convertedSize = {ySize,xSize};
        return convertedSize;
    }
    public void RevealItemInfo(Item _item)
    {
        if (_item == null)
            return;
        itemInfo.SetActive(true);
        Vector3 offset = new Vector3(-200, 200);
        RectTransform infoRect = itemInfo.GetComponent<RectTransform>();
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
        itemInfoTexts[0].text = $"{_item.itemName}";
        itemInfoTexts[1].text = $"{_item.itemType}";
        itemInfoTexts[2].text = $"{_item.itemRarity}";
        if (equipFlag == true)
            itemInfoTexts[3].text = $"{_item.equipPart}";
        else
            itemInfoTexts[3].text = "";
        if(stat1 != float.NaN)
        {
            itemInfoTexts[4].text = $"{stat1Name} : {stat1}";
            itemInfoTexts[5].text = $"MoveSpeed : {stat2}";
        }
        itemInfoTexts[6].text = $"{_item.itemText}";
    }
    public void ConcealItemInfo()
    {
        itemInfo.SetActive(false);
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
}
