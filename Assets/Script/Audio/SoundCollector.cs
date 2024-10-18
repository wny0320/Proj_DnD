using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundCollector")]
public class SoundCollector : ScriptableObject
{
    //이쪽에 사운드 넣으면 됨
    public AudioClip hitClip;

    [Header("주먹")]
    public AudioClip punching;
    public AudioClip potionDrinking;

    //onehanded
    [Header("한손검")]
    public AudioClip onehandedSwing1;
    public AudioClip onehandedSwing2;
    public AudioClip onehandedStab;

    //twohanded
    [Header("양손검")]
    public AudioClip twohandedStab;
    public AudioClip twohandedSwing;

    [Header("사망")]
    public AudioClip PlayerDead;
    public AudioClip SkeletonDead;
    public AudioClip WatcherDead;
    public AudioClip MinotaurDead;
    public AudioClip CrusaderDead;
    public AudioClip BanditDead;

    [Header("상인 소리")]
    public AudioClip ShopEnter;
    public AudioClip ShopCoin;

    [Header("Props 관련")]
    public AudioClip DoorOpen;
    public AudioClip DoorClose;
    public AudioClip TorchOn;
    public AudioClip TorchOff;
    public AudioClip BoxBreak;
}
