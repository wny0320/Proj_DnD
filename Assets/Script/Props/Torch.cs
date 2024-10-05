using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> torchLightList;
    private bool lightOnFlag = true;

    public void LightChange()
    {
        int count = torchLightList.Count;
        if (lightOnFlag == true)
        {
            for(int i = 0; i < count; i++)
            {
                torchLightList[i].SetActive(false);
            }
            lightOnFlag = false;
            Global.sfx.Play(Global.Sound.TorchOff, transform.position);
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                torchLightList[i].SetActive(true);
            }
            lightOnFlag = true;
            Global.sfx.Play(Global.Sound.TorchOn, transform.position);
        }
    }
}
