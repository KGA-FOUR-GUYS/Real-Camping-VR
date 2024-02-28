using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddFireDuration : MonoBehaviour
{
    [SerializeField] ParticleSystem[] FirePS;

    private void Awake()
    {
        FirePS = GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            for (int i = 0; i < FirePS.Length; i++)
            {
                FirePS[i].Play();
            }
        }
    }
}
