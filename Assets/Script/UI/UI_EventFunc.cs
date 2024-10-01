using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Reflection;
using System;
using UnityEngine.EventSystems;

public class UI_EventFunc : MonoBehaviour
{
    Dictionary<string, MethodInfo> buttonMethods = new Dictionary<string, MethodInfo>();
    //버튼에 들어갈 함수들
    private Button button;
    [SerializeField]
    private ButtonFunc buttonFunc;
    [Header("Plz Assgin Only Need Data")]
    [Header("Lobby Button Resources")]
    [SerializeField]
    GameObject merchantObject;
    [SerializeField]
    GameObject potionObject;
    [SerializeField]
    GameObject armorObject;
    [SerializeField]
    GameObject sellObject;
    [Header("ShopUI Button Resources")]
    [SerializeField]
    ShopUI shopUI;
    private void Awake()
    {
        GetButtonFunc();
    }
    private void Start()
    {
        DataSet();
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            buttonMethods[buttonFunc.ToString()]?.Invoke(this, null);
        });
    }
    private void GetButtonFunc()
    {
        foreach (var method in typeof(UI_EventFunc).GetMethods())
        {
            if (method.IsPublic == true)
            {
                if (buttonMethods.ContainsKey(method.Name))
                    continue;
                buttonMethods.Add(method.Name, method);
            }
        }
        //해당 타입에서 모든 메소드를 가져오는 코드
        //https://stackoverflow.com/questions/1198417/generate-list-of-methods-of-a-class-with-method-types
        //foreach (var method in typeof(Boss1PatternState).GetMethods())
        //{
        //    var parameters = method.GetParameters();
        //    var parameterDescriptions = parameters.Select(x => x.ParameterType + " " + x.Name).ToArray();
        //}
        //코루틴을 가져와서 실행하는 자료
        //https://discussions.unity.com/t/get-ienumerator-from-name-or-convert-methodinfo-to-ienumerator/155580/2
    }
    private void DataSet()
    {
        if(merchantObject != null) merchantObject.SetActive(false);
    }
    public void MerchantButton()
    {
        if (merchantObject == null)
            return;
        Manager.Inven.RevealInvenCanvasByBt();
        merchantObject.SetActive(true);
        Manager.Inven.ConcealStashCanvas();
    }
    public void AdventureButton()
    {
        if (merchantObject == null)
            return;
        Manager.Inven.ConcealInvenCanvasByBt();
        merchantObject.SetActive(false);
        Manager.Inven.ConcealStashCanvas();
    }
    public void StashButton()
    {
        if (merchantObject == null)
            return;
        Manager.Inven.RevealInvenCanvasByBt();
        merchantObject.SetActive(false);
        Manager.Inven.RevealStashCanvas();
    }

    public void MerchantConsumButton()
    {
        if (shopUI == null) return;
        shopUI.ActiveConsumUI();
    }
    public void MerchantEquipButton()
    {
        if (shopUI == null) return;
        shopUI.ActiveEquipUI();
    }
    public void MerchantSellButton()
    {
        if (shopUI == null) return;
        shopUI.ActiveSellUI();
    }
}
