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
    //��ư�� �� �Լ���
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
    [SerializeField]
    GameObject specialObject;
    [Header("ShopUI Button Resources")]
    [SerializeField]
    ShopUI shopUI;

    AudioSource audioSource;
    private void Awake()
    {
        GetButtonFunc();
        audioSource = GetComponent<AudioSource>();
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
        //�ش� Ÿ�Կ��� ��� �޼ҵ带 �������� �ڵ�
        //https://stackoverflow.com/questions/1198417/generate-list-of-methods-of-a-class-with-method-types
        //foreach (var method in typeof(Boss1PatternState).GetMethods())
        //{
        //    var parameters = method.GetParameters();
        //    var parameterDescriptions = parameters.Select(x => x.ParameterType + " " + x.Name).ToArray();
        //}
        //�ڷ�ƾ�� �����ͼ� �����ϴ� �ڷ�
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
        if (merchantObject.activeSelf == true)
            return;
        Manager.Inven.RevealInvenCanvasByBt();
        merchantObject.SetActive(true);
        Manager.Inven.ConcealStashCanvas();
        audioSource.clip = Global.Sound.ShopEnter;
        audioSource.Play();
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
    public void MerchantSpecialButton()
    {
        if (shopUI == null) return;
        shopUI.ActiveSpecialUI();
    }
}
