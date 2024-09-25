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

    //무기 및 방어구
    public static Weapon PlayerWeapon;
    public static Action<Item> PlayerWeaponEquip;
    public static Action<Item> PlayerWeaponUnEquip;
    public static Action<Item> PlayerArmorEquip;
    public static Action<Slot> PlayerArmorUnEquip;
}
