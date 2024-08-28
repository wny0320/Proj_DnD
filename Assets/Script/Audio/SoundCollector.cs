using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundCollector")]
public class SoundCollector : ScriptableObject
{
    //���ʿ� ���� ������ ��
    public AudioClip hitClip;
    public AudioClip breakBoxClip;

    //onehanded
    public AudioClip onehandedSwing1;
    public AudioClip onehandedSwing2;
    public AudioClip onehandedStab;

    //twohanded
    public AudioClip twohandedStab;
    public AudioClip twohandedSwing;
}
