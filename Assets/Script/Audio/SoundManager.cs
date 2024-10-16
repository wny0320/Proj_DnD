using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    private const string Master = "MASTER";
    private const string Bgm = "BGM";
    private const string Effect = "EFFECT";

    //볼륨 조절, slider 최소값 0.0001, 최대값 1
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider effectSlider;
    public GameObject settingUI;

    //효과음 관련    
    [SerializeField] private GameObject sfxInstance;
    private Transform sfxParent;
    private List<SoundEffect> pool = new();

    //BGM 관련 나중에 추가 예정

    private void Awake()
    {
        settingUI.SetActive(false);
        Global.sfx = this;
        sfxParent = new GameObject("Sfx").transform;
        sfxParent.SetParent(transform);
        DontDestroyOnLoad(this);
        Init();
    }
    private void FixedUpdate()
    {
        OnESC();
    }

    public void Play(AudioClip clip)
    {
        Get().Play(clip);
    }

    public void Play(AudioClip clip, Vector3 position)
    {
        SoundEffect se = Get();
        se.transform.position = position;
        se.Play(clip);
    }
    private void OnESC()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(settingUI.activeSelf == true)
            {
                settingUI.SetActive(false);
                if (Manager.Instance.GetNowScene().name == SceneName.DungeonScene.ToString())
                    Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                settingUI.SetActive(true);
                if (Manager.Instance.GetNowScene().name == SceneName.DungeonScene.ToString())
                    Cursor.lockState = CursorLockMode.None;
            }
        }
    }
    private SoundEffect Get()
    {
        foreach (SoundEffect sfx in pool) if (sfx.isFree) return sfx;
        return Create();
    }

    private SoundEffect Create()
    {
        SoundEffect instance = Instantiate(sfxInstance, sfxParent).GetComponent<SoundEffect>();
        pool.Add(instance);
        return instance;
    }

    private void Init()
    {
        //playerprefs로 값 저장해도 좋을 듯
        float value;
        audioMixer.GetFloat(Master, out value);
        masterSlider.value = Mathf.Pow(10, value / 20);
        audioMixer.GetFloat(Bgm, out value);
        bgmSlider.value = Mathf.Pow(10, value / 20);
        audioMixer.GetFloat(Effect, out value);
        effectSlider.value = Mathf.Pow(10, value / 20);

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBgmVolume);
        effectSlider.onValueChanged.AddListener(SetEffectVolume);
    }

    public void SetMasterVolume(float value) => audioMixer.SetFloat(Master, Mathf.Log10(value) * 20);
    public void SetBgmVolume(float value) => audioMixer.SetFloat(Bgm, Mathf.Log10(value) * 20);
    public void SetEffectVolume(float value) => audioMixer.SetFloat(Effect, Mathf.Log10(value) * 20);
}