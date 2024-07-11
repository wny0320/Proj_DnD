using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
    //이곳에 전역 이벤트 & 변수 정의
    //Action, Func
    //EX)

    //AssetCollector
    public static SoundCollector Sound;
    public static EffectCollector Effect;

    //SFX 사용법 : Global.sfx.Play(Global.Sound.testclip);
    public static SoundManager sfx;
    public static EffectManager fx;

    //무기
    public static Weapon PlayerWeapon;

    //플레이어 생성시 뿌려줄 것
    //public static Action<GameObject> PlayerSetted;
}
