using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class DataManager
{
    #region ItemData
    //������ �Ŵ��� - ������ ����
    public Dictionary<string, Item> itemData = new Dictionary<string, Item>();
    //������ �����Ϳ� ����ִ� Ű ������� �̹��� ���� �� �־ itemIamges�� index�� itemImageNum�� ������ �� 
    //�ش� �̹��� ��� ���������� ���
    public List<Sprite> itemSprite = new List<Sprite>();
    public List<GameObject> itemUIPrefab = new List<GameObject>();
    public List<GameObject> item3DPrefab = new List<GameObject>();
    public bool dataImportFlag = false;
    const string ITEM_PATH = "Items/";
    #endregion
    public int gold = 0;
    public void OnAwake()
    {
        SaveItemDataToJson();
        ItemDataImport();

    }
    public string JsonSerialize(object _obj)
    {
        return JsonConvert.SerializeObject(_obj, Formatting.Indented);
    }
    public object JsonDeserialize(string _jsonDatam, Type _type)
    {
        return JsonConvert.DeserializeObject(_jsonDatam, _type);
    }
    public void SaveItemDataToJson()
    {
        string itemJsonPath = Path.Combine(Application.persistentDataPath, "itemData.Json");
        // �ش� �κ��� ������ ������ ������Ʈ ���ϰ� �ϴ°ɷ� ���ҽ� �Ƴ��� ������ �߰�
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
        {
            itemData.Add(item.itemName, item);
        }
        string json = JsonSerialize(itemData);
        File.WriteAllText(itemJsonPath, json);
        Debug.Log(itemJsonPath);
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
    }
}
