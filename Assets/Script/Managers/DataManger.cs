using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager
{
    //데이터 매니저 - 데이터 관련
    Dictionary<string, Dictionary<string, float>> itemData = new Dictionary<string, Dictionary<string, float>>();
    public void OnStart()
    {
        //데이터 들어가는 형식 확인용
        //Dictionary<string, float> statDict = new Dictionary<string, float>();
        //statDict.Add("hp", 1);
        //itemData.Add("itemName", statDict);
        //Json 아이템 데이터 형식
        // {"itemName_Key":{"Hp":1,"MaxHp":2,"Attack":0,"Defense":0,"MoveSpeed":0.0,"AttackSpeed":0.0,"JumpForce":0.0}}
        GetItemJson();
    }
    public string JsonSerialize(object _obj)
    {
        return JsonConvert.SerializeObject(_obj);
    }
    public object JsonDeserialize(string _jsonDatam, Type _type)
    {
        return JsonConvert.DeserializeObject(_jsonDatam, _type);
    }
    public void GetItemJson()
    {
        string itemJsonPath = Path.Combine(Application.persistentDataPath, "itemData.Json");
        if(File.Exists(itemJsonPath))
        {
            string json = File.ReadAllText(itemJsonPath);
            itemData = (Dictionary<string, Dictionary<string, float>>)JsonDeserialize(json, typeof(Dictionary<string, Dictionary<string, float>>));
        }
        else
        {
            string json = JsonSerialize(itemData);
            File.WriteAllText(itemJsonPath, json);
        }
        Debug.Log(itemJsonPath);
    }
    public void InsertItemData()
    {
        
    }
}
