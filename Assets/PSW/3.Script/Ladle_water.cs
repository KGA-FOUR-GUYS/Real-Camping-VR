using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladle_water : MonoBehaviour
{
    [SerializeField] GameObject Ladle_Water;
    [SerializeField] ParticleSystem Water_Particle;
    [SerializeField] float Water_make_delay;
    [SerializeField] float Water_destroy_delay;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water Volume"))
        {
            Invoke(nameof(Water_On), Water_make_delay);
        }
    }

    private void Water_On()
    {
        Ladle_Water.SetActive(true);
    }
    private void Water_Off()
    {
        Ladle_Water.SetActive(false);
        Water_Particle.Stop();
    }

    //Ontilt시스템에서 사용
    public void Water_Fall_Start()
    {
        if (Ladle_Water.activeInHierarchy)
        {
            Water_Particle.Play();
            Water_Fall_End();
        }
    }
    private void Water_Fall_End()
    {
        if (Water_Particle.isPlaying)
        {
            Invoke(nameof(Water_Off), Water_destroy_delay);
        }
    }
}
