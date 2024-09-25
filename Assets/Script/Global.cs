using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
    //�̰��� ���� �̺�Ʈ & ���� ����
    //Action, Func
    //EX)

    //AssetCollector
    public static SoundCollector Sound;
    public static EffectCollector Effect;

    //SFX ���� : Global.sfx.Play(Global.Sound.testclip);
    public static SoundManager sfx;
    public static EffectManager fx;

    //���� �� ��
    public static Weapon PlayerWeapon;
    public static Action<Item> PlayerWeaponEquip;
    public static Action<Item> PlayerWeaponUnEquip;
    public static Action<Item> PlayerArmorEquip;
    public static Action<Slot> PlayerArmorUnEquip;
}
