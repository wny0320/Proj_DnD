using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetCollector : MonoBehaviour
{
    //사운드, 이펙트 넣을 곳
    public SoundCollector sound;
    public EffectCollector effect;

    private void Awake()
    {
        Global.Sound = sound;
        Global.Effect = effect;
    }
}