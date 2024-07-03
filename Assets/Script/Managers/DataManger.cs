using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class DataManager
{
    //������ �Ŵ��� - ������ ����
    Dictionary<string, Item> itemData = new Dictionary<string, Item>();
    //������ �����Ϳ� ����ִ� Ű ������� �̹��� ���� �� �־ itemIamges�� index�� itemImageNum�� ������ �� 
    List<Sprite> itemImages = new List<Sprite>();
    const string ITEM_PATH = "Items/";
    public void OnAwake()
    {
        GetItemDataJson();
        ItemImageImport();

    }
    public string JsonSerialize(object _obj)
    {
        return JsonConvert.SerializeObject(_obj, Formatting.Indented);
    }
    public object JsonDeserialize(string _jsonDatam, Type _type)
    {
        return JsonConvert.DeserializeObject(_jsonDatam, _type);
    }
    public void GetItemDataJson()
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
    public void ItemImageImport()
    {
        int index = 0;
        foreach(string itemName in itemData.Keys)
        {
            itemData[itemName].itemImageNum = index;
            string targetImagePath = ITEM_PATH + "ItemImages/" + itemName + "Image";
            Sprite targetItemImage = Resources.Load<Sprite>(targetImagePath);
            itemImages.Add(targetItemImage);
            index++;
        }
    }
}
