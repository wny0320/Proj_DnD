using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundCollector")]
public class SoundCollector : ScriptableObject
{
    //���ʿ� ���� ������ ��
    public AudioClip hitClip;

    [Header("�ָ�")]
    public AudioClip punching;
    public AudioClip potionDrinking;

    //onehanded
    [Header("�Ѽհ�")]
    public AudioClip onehandedSwing1;
    public AudioClip onehandedSwing2;
    public AudioClip onehandedStab;

    //twohanded
    [Header("��հ�")]
    public AudioClip twohandedStab;
    public AudioClip twohandedSwing;

    [Header("���")]
    public AudioClip PlayerDead;
    public AudioClip SkeletonDead;
    public AudioClip WatcherDead;
    public AudioClip MinotaurDead;
    public AudioClip CrusaderDead;
    public AudioClip BanditDead;

    [Header("���� �Ҹ�")]
    public AudioClip ShopEnter;
    public AudioClip ShopCoin;

    [Header("Props ����")]
    public AudioClip DoorOpen;
    public AudioClip DoorClose;
    public AudioClip TorchOn;
    public AudioClip TorchOff;
    public AudioClip BoxBreak;
}
