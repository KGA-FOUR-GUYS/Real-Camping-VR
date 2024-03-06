using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCollider : MonoBehaviour
{
    [SerializeField] ParticleSystem FirePS;
    [SerializeField] BoxCollider FireBox;

    private void Awake()
    {
        FirePS = GetComponentInChildren<ParticleSystem>();
        FireBox = this.GetComponent<BoxCollider>();
    }

    private void Start()
    {
        StartCoroutine(ColliderTrigger_Co());
    }

    IEnumerator ColliderTrigger_Co()
    {
        if (!FireBox.enabled)
        {
            while (this.gameObject.activeInHierarchy)
            {
                FireBox.enabled = FirePS.isPlaying;
                yield return null;
            }
            yield return null;
        }
    }
}
