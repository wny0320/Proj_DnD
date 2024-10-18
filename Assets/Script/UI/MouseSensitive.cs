using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class MouseSensitive : MonoBehaviour
{
    const string Mouse = "MOUSE";

    public Slider sensitiveSlider;

    private void Start()
    {
        SetPlayerPrefs();
        Init();
    }

    private void SetPlayerPrefs()
    {
        if (PlayerPrefs.HasKey(Mouse)) return;

        PlayerPrefs.SetFloat(Mouse, 2.5f);
    }

    private void Init()
    {
        sensitiveSlider.value = PlayerPrefs.GetFloat(Mouse);

        SetMouseSensitive(sensitiveSlider.value);

        sensitiveSlider.onValueChanged.AddListener(SetMouseSensitive);
    }

    public void SetMouseSensitive(float value)
    {
        Manager.Input.mouseSpeed = value;
        PlayerPrefs.SetFloat(Mouse, value);
    }
}
