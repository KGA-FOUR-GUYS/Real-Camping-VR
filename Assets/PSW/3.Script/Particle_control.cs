using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_control : MonoBehaviour
{
    ParticleSystem Particle;
    [SerializeField] int layer_num;

    void Start()
    {
        Particle = this.GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.layer == layer_num)
        {

        }


    }



}
