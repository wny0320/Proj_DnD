using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectCollector")]
public class EffectCollector : ScriptableObject
{
    //이곳에 이펙트 넣음 됨
    public GameObject hiteffect;


    public GameObject GetEffect(EffectType type)
    {
        //이펙트 가져오기
        switch(type)
        {
            case EffectType.Blood:
                return hiteffect;
        }

        return null;
    }
}
