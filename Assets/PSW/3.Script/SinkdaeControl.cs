using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkdaeControl : MonoBehaviour
{
    [SerializeField] ParticleSystem water_ps;
    [SerializeField] int targer_layer_num = 0;
    MeshRenderer meshRenderer;    
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.color = Color.grey;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == targer_layer_num)
        {
            StateChange();
        }
    }

    public void StateChange()
    {
        if (meshRenderer.material.color == Color.grey)
        {
            meshRenderer.material.color = Color.cyan;
        }
        else
        {
            meshRenderer.material.color = Color.grey;
        }        
        if (water_ps.isPlaying)
        {
            water_ps.Stop();
        }
        else
        {
            water_ps.Play();
        }
    }
}
