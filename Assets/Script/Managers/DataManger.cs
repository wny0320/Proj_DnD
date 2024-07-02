using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class DataManager
{
    //데이터 매니저 - 데이터 관련
    Dictionary<string, Item> itemData = new Dictionary<string, Item>();
    public void OnStart()
    {
        GetItemDataJson();
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
        if (File.Exists(itemJsonPath))
        {
            string json = File.ReadAllText(itemJsonPath);
            itemData = (Dictionary<string, Item>)JsonDeserialize(json, typeof(Dictionary<string, Item>));
        }
        else
        {
            string json = JsonSerialize(itemData);
            File.WriteAllText(itemJsonPath, json);
        }
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
}
