using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_control : MonoBehaviour
{
    int count = 0;
    [SerializeField] string Required_Tag = string.Empty;
    [SerializeField] int collision_count;

    void OnParticleCollision(GameObject other)
    {
        if (CanTrigger(other.gameObject))
        {
            if (!other.transform.GetChild(0).gameObject.activeInHierarchy)
            {
                count++;
            }

            if (count >= collision_count)
            {
                other.transform.GetChild(0).gameObject.SetActive(true);
                count = 0;
            }
        }
    }

    bool CanTrigger(GameObject otherGameObject)
    {
        if (Required_Tag != string.Empty)
            return otherGameObject.CompareTag(Required_Tag);
        else
            return true;
    }


}
