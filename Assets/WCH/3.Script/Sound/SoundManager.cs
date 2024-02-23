using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    private int count = 0;

    private AudioSource[] audioSources;

    public AudioClip[] SFX;
    public AudioClip[] Cooking_SFX;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSources = GetComponentsInChildren<AudioSource>();
    }


    public void PlaySFX(int index)
    {
        if (count >= audioSources.Length)
        {
            count = 0;
        }

        audioSources[count].clip = SFX[index];

        audioSources[count].Play();

        count++;
    }
    public void PlayCookingSFX(int index)
    {
        if (count >= audioSources.Length)
        {
            count = 0;
        }

        audioSources[count].clip = Cooking_SFX[index];

        audioSources[count].Play();

        count++;
    }
}
