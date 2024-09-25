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
    [Header("Lobby Button Resource")]
    [SerializeField]
    GameObject merchantObject;
    [SerializeField]
    GameObject stashObject;
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
        if(stashObject != null) stashObject.SetActive(false);
    }
    public void MerchantButton()
    {
        if(merchantObject == null)
        {
            Debug.LogError("MerchantCanvas Is Not Assigned");
            return;
        }
        if(stashObject == null)
        {
            Debug.LogError("StashCanvas Is Not Assigned");
            return;
        }
        Manager.Inven.RevealInvenCanvasByBt();
        merchantObject.SetActive(true);
        stashObject.SetActive(false);
    }
    public void AdventureButton()
    {
        if (merchantObject == null)
        {
            Debug.LogError("MerchantCanvas Is Not Assigned");
            return;
        }
        if (stashObject == null)
        {
            Debug.LogError("StashCanvas Is Not Assigned");
            return;
        }
        Manager.Inven.ConcealInvenCanvasByBt();
        merchantObject.SetActive(false);
        stashObject.SetActive(false);
    }
    public void StashButton()
    {
        if (merchantObject == null)
        {
            Debug.LogError("MerchantCanvas Is Not Assigned");
            return;
        }
        if (stashObject == null)
        {
            Debug.LogError("StashCanvas Is Not Assigned");
            return;
        }
        Manager.Inven.RevealInvenCanvasByBt();
        merchantObject.SetActive(false);
        stashObject.SetActive(true);
    }

    public void MerchantTradeButton()
    {

    }
}
