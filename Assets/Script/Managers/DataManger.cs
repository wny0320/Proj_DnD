using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class DataManager
{
    #region ItemData
    //데이터 매니저 - 데이터 관련
    public Dictionary<string, Item> itemData = new Dictionary<string, Item>();
    //아이템 데이터에 들어있는 키 순서대로 이미지 또한 들어가 있어서 itemIamges의 index를 itemImageNum에 넣으면 됨 
    //해당 이미지 대신 프리팹으로 사용
    public List<Sprite> itemSprite = new List<Sprite>();
    public List<GameObject> itemUIPrefab = new List<GameObject>();
    public List<GameObject> item3DPrefab = new List<GameObject>();
    public bool dataImportFlag = false;
    const string ITEM_PATH = "Items/";
    #endregion
    public int gold = 1000;
    public void OnAwake()
    {
        ItemDataResourceLoad();
        ItemDataImport();
    }
    public void OnStart()
    {
        PlayerDataImport();
        Manager.Inven.ConcealStashCanvas();
    }
    public string JsonSerialize(object _obj)
    {
        return JsonConvert.SerializeObject(_obj, Formatting.Indented);
    }
    public object JsonDeserialize(string _jsonData, Type _type)
    {
        return JsonConvert.DeserializeObject(_jsonData, _type);
    }
    public void ItemDataResourceLoad()
    {
        // 해당 부분은 파일이 있을때 업데이트 안하게 하는걸로 리소스 아끼고 싶을때 추가
        //if (File.Exists(itemJsonPath))
        //{
        //    string json = File.ReadAllText(itemJsonPath);
        //    Dictionary<string, Item> existData = (Dictionary<string, Item>)JsonDeserialize(json, typeof(Dictionary<string, Item>));
        //}
        //else
        //{
        //    Item[] items = Resources.LoadAll<Item>(ITEM_PATH);
        //    foreach (Item item in items)
        //    {

        //    }
        //    string json = JsonSerialize(itemData);
        //    File.WriteAllText(itemJsonPath, json);
        //}
        Item[] items = Resources.LoadAll<Item>(ITEM_PATH);
        foreach (Item item in items)
            itemData.Add(item.itemName, item);
    }
    public void SaveItemJson(string _json)
    {
        string itemJsonPath = Path.Combine(Application.persistentDataPath, "itemData.Json");
        File.WriteAllText(itemJsonPath, _json);
    }
    //public void InsertTestItemData()
    //{
    //    Dictionary<string,Item> exampleDict = new Dictionary<string,Item>();
    //    Stat exampleStat = new Stat(1,2);
    //    Item exampleItem = new Item();
    //    exampleItem.name = "exampleItem";
    //    exampleItem.itemImageNum = 0;
    //    exampleItem.itemStat = exampleStat;
    //    exampleItem.equipPart = EquipPart.Chest;
    //    exampleItem.itemSize = ItemSize.slot2x3;
    //    exampleItem.itemMaxStack = 0;
    //    exampleItem.itemStack = 0;
    //    exampleDict.Add(exampleItem.name, exampleItem);
    //    string exampleJson = JsonSerialize(exampleDict);
    //    SaveItemJson(exampleJson);
    //}
    public Item GetItemFromJson(string _itemName)
    {
        return itemData[_itemName];
    }
    public void ItemDataImport()
    {
        int index = 0;
        foreach(string itemName in itemData.Keys)
        {
            itemData[itemName].itemIndex = index;
            string targetSpritePath = ITEM_PATH + "ItemUISprite/" + itemName + "Sprite";
            string targetUIPrefabPath = ITEM_PATH + "ItemUIPrefab/" + itemName + "Prefab";
            string target3DPrefabPath = ITEM_PATH + "Item3DPrefab/" + itemName + "Prefab";
            Sprite targetItemImage = Resources.Load<Sprite>(targetSpritePath);
            GameObject targetItemUIPrefab = Resources.Load<GameObject>(targetUIPrefabPath);
            GameObject targetItem3DPrefab = Resources.Load<GameObject>(target3DPrefabPath);
            itemSprite.Add(targetItemImage);
            itemUIPrefab.Add(targetItemUIPrefab);
            item3DPrefab.Add(targetItem3DPrefab);
            index++;
        }
        dataImportFlag = true;
        string itemJsonPath = Path.Combine(Application.persistentDataPath, "itemData.Json");
        string json = JsonSerialize(itemData);
        File.WriteAllText(itemJsonPath, json);
        Debug.Log(itemJsonPath);
    }
    public void PlayerDataExport()
    {
        SlotJsonClass slotJsonClass = new SlotJsonClass();
        PlayerDataTransfer(slotJsonClass, Manager.Inven.invenSlotLines, ItemBoxType.Inventory);
        PlayerDataTransfer(slotJsonClass, Manager.Inven.stashSlotLines, ItemBoxType.Stash);
        PlayerDataTransfer(slotJsonClass, Manager.Inven.equipSlots);
        string playerItemDataJson = JsonSerialize(slotJsonClass);
        string jsonPath = Path.Combine(Application.persistentDataPath, "playerItemData.Json");
        File.WriteAllText(jsonPath, playerItemDataJson);
    }
    public void PlayerDataImport()
    {
        string jsonPath = Path.Combine(Application.persistentDataPath, "playerItemData.Json");
        if(File.Exists(jsonPath) == true)
        {
            string jsonFile = File.ReadAllText(jsonPath);
            SlotJsonClass slotJsonClass = (SlotJsonClass)JsonDeserialize(jsonFile, typeof(SlotJsonClass));
            Manager.Inven.RecoverPlayerItemData(slotJsonClass);
        }
    }
    public void PlayerDataTransfer(SlotJsonClass _slotJsonClass, List<SlotLine> _targetSlotLines, ItemBoxType _itemBoxType)
    {
        List<JsonSlotLine> jsonSlotLines = new List<JsonSlotLine>();
        int cnt = _targetSlotLines.Count;
        for(int y = 0; y < cnt; y++)
        {
            JsonSlotLine jsonSlotLine = new JsonSlotLine();
            jsonSlotLines.Add(jsonSlotLine);
            for(int x = 0;  x < _targetSlotLines[y].mySlots.Count; x++)
            {
                JsonSlot jsonSlot = new JsonSlot();
                jsonSlot = jsonSlot.SlotToJsonSlot(_targetSlotLines[y].mySlots[x]);
                jsonSlotLines[y].mySlots.Add(jsonSlot);
            }
        }
        switch(_itemBoxType)
        {
            case ItemBoxType.Inventory:
                _slotJsonClass.invenSlotLines = jsonSlotLines;
                break;
            case ItemBoxType.Stash:
                _slotJsonClass.stashSlotLines = jsonSlotLines;
                break;
            default:
                break;
        }
    }
    public void PlayerDataTransfer(SlotJsonClass _slotJsonClass, Dictionary<string, Slot> _targetSlots)
    {
        Dictionary<string, JsonSlot> jsonSlots = new Dictionary<string, JsonSlot>();
        foreach(string key in _targetSlots.Keys)
        {
            JsonSlot jsonSlot = new JsonSlot();
            jsonSlot.SlotToJsonSlot(_targetSlots[key]);
            jsonSlots.Add(key, jsonSlot);
        }
        _slotJsonClass.equipSlotDict = jsonSlots;
    }
}
