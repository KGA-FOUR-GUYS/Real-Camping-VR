using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    public enum SFXName
    {

    }
    private int count = 0;

    private AudioSource[] audioSources;

    [Header("")]
    public AudioClip[] SFX;

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


    public void PlaySound()
    {
        if (count >= audioSources.Length - 1)
        {
            count = 0;
        }


        count++;
    }


}
