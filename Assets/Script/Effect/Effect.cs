using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public bool isFree = true;
    private ParticleSystem particle;

    public void Play()
    {
        if (!isFree) return;

        gameObject.SetActive(true);
        isFree = false;
        if (particle == null) particle = GetComponent<ParticleSystem>();

        particle.Play();
        StartCoroutine(Timer());
    }

    private void Stop()
    {
        isFree = true;
        gameObject.SetActive(false);
    }

    private IEnumerator Timer()
    {
        while(true)
        {
            yield return null;
            if (!particle.IsAlive()) break;
        }

        Stop();
    }
}