using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldUI : MonoBehaviour
{
    public Text GoldText;
    // Update is called once per frame
    void Update()
    {
        GoldText.text = Manager.Data.gold.ToString();
    }
}
