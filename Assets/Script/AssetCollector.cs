using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetCollector : MonoBehaviour
{
    //����, ����Ʈ ���� ��
    public SoundCollector sound;
    public EffectCollector effect;

    private void Awake()
    {
        Global.Sound = sound;
        Global.Effect = effect;
    }
}