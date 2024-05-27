using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectCollector")]
public class EffectCollector : ScriptableObject
{
    //�̰��� ����Ʈ ���� ��
    public GameObject hiteffect;


    public GameObject GetEffect(EffectType type)
    {
        //����Ʈ ��������
        switch(type)
        {
            case EffectType.Blood:
                return hiteffect;
        }

        return null;
    }
}
