using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkdaeControl : MonoBehaviour
{
    [SerializeField] ParticleSystem water_ps;
    [SerializeField] Material Target_Mat;
    [SerializeField] int targer_layer_num = 0;
    Color cur_color;
    private void Awake()
    {
        Target_Mat.color = Color.grey;        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == targer_layer_num)
        {
            StateChange();
        }
    }

    public void StateChange()
    {
        if (Target_Mat.color == cur_color)
        {
            Target_Mat.color = Color.cyan;
        }
        else
        {
            Target_Mat.color = Color.grey;
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
