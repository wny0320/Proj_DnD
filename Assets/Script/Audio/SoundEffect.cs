using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public bool isFree = true;
    private AudioSource audioSource;

    public void Play(AudioClip clip)
    {
        if (!isFree) return;

        gameObject.SetActive(true);
        isFree = false;
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        audioSource.clip = clip;
        audioSource.Play();
        StartCoroutine(Timer(clip.length));
    }

    private void Stop()
    {
        audioSource.Stop();
        isFree = true;
        gameObject.SetActive(false);
    }

    private IEnumerator Timer(float t)
    {
        yield return new WaitForSecondsRealtime(t);
        Stop();
    }
}