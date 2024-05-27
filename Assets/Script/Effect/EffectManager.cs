using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

public class EffectManager : MonoBehaviour
{
    private Transform fxParent;
    private Dictionary<EffectType ,List<Effect>> pool = new();

    //BGM 관련 나중에 추가 예정

    private void Awake()
    {
        Global.fx = this;
        fxParent = new GameObject("Fx").transform;
        fxParent.SetParent(transform);

        foreach(EffectType type in Enum.GetValues(typeof(EffectType)))
            pool[type] = new List<Effect>();
    }

    public void Play(EffectType type, Vector3 position)
    {
        Effect se = Get(type);
        se.transform.position = position;
        se.Play();
    }

    private Effect Get(EffectType type)
    {
        foreach(Effect effect in pool[type]) if (effect.isFree) return effect;

        return Create(type);
    }

    private Effect Create(EffectType type)
    {
        GameObject go = Global.Effect.GetEffect(type);
        Effect instance = Instantiate(go, fxParent).GetComponent<Effect>();
        pool[type].Add(instance);
        return instance;
    }
}