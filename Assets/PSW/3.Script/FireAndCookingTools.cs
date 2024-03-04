using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyWaterBuoyancy;
using Cooking;

public class FireAndCookingTools : MonoBehaviour
{
    [SerializeField] ParticleSystem FirePS;
    [SerializeField] BoxCollider FireBox;
    [SerializeField] WaterFX water;
    [SerializeField] MeshCollider BroilArea;
    [SerializeField] MeshCollider BoilArea;

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

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Dish"))
        {
            //���� �ڽĿ� ���ų� ���������� BroilȰ��ȭ & Boil����
            water = other.transform.GetComponentInChildren<WaterFX>();
            BroilArea = other.transform.GetComponentInChildren<BroilManager>().GetComponent<MeshCollider>();
            BoilArea = other.transform.GetComponentInChildren<BoilManager>().GetComponent<MeshCollider>();
            if (water == null || !water.gameObject.activeInHierarchy)
            {
                BroilArea.enabled = true;
                if (water != null)
                {
                    BoilArea.enabled = false;
                }
            }
            //���� �ڽĿ� �ְ� ���������� BoilȰ��ȭ & Broil����
            if (water != null && water.gameObject.activeInHierarchy)
            {
                BoilArea.enabled = true;
                if (BroilArea != null)
                {
                    BroilArea.enabled = false;
                }
            }
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Dish"))
        {
            BroilArea = other.transform.GetComponentInChildren<BroilManager>().GetComponent<MeshCollider>();
            BoilArea = other.transform.GetComponentInChildren<BoilManager>().GetComponent<MeshCollider>();
            if (BroilArea != null)
            {
                BroilArea.enabled = false;
            }
            if (BoilArea != null)
            {
                BoilArea.enabled = false;
            }
        }
    }
}
