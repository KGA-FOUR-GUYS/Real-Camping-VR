using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SinkdaeControl : NetworkBehaviour
{
    [SerializeField] ParticleSystem water_ps;
    [SerializeField] int targer_layer_num = 0;
    MeshRenderer meshRenderer;
    
    [SyncVar(hook = nameof(SyncDelay))]
    public float delay = 1f;
    private void SyncDelay(float _, float newValue)
    {
        delay = newValue;
    }

    [SyncVar(hook = nameof(SyncCoolDown))]
    public float coolDown = 0f;
    private void SyncCoolDown(float _, float newValue)
    {
        coolDown = newValue;
    }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.color = Color.grey;
    }

    private void Update()
    {
        coolDown = Mathf.Max(0, coolDown - Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isServer) return;
        if (coolDown > 0) return;

        if (other.gameObject.layer == targer_layer_num)
        {
            coolDown = delay;
            CmdChangeState();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdChangeState()
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

        RpcChangeState();
    }

    [ClientRpc]
    public void RpcChangeState()
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
