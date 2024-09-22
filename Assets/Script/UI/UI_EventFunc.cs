using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Reflection;
using System;

public class UI_EventFunc : MonoBehaviour
{
    Dictionary<string, MethodInfo> buttonMethods = new Dictionary<string, MethodInfo>();
    //버튼에 들어갈 함수들
    private Button button;
    [SerializeField]
    private ButtonFunc buttonFunc;
    private void Awake()
    {
        GetButtonFunc();
    }
    private void Start()
    {
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
    public void MerchantButton()
    {
        const string MERCHANT_PATH = "MerchantCanvas";
        const string STASH_PATH = "StashCanvas";
        GameObject merchantObject = GameObject.Find(MERCHANT_PATH);
        string activeButtonName = GetComponent<Button>().name;
        switch(activeButtonName)
        {
            case "Adventure":
                break;
            case "Merchant":
                break;
            case "Stash":
                break;
            default:
                Debug.LogError("Assgined Button Name");
                return;
        }
        merchantObject.SetActive(true);
    }
}
