using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundCollector")]
public class SoundCollector : ScriptableObject
{
    //이쪽에 사운드 넣으면 됨
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
