using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_control : MonoBehaviour
{
    [SerializeField] string Required_Tag = string.Empty;
    [SerializeField] int collision_count;
    [SerializeField]int count = 0;

    void OnParticleCollision(GameObject other)
    {
        if (CanTrigger(other.gameObject))
        {
            if (!other.transform.Find("Water_Box").gameObject.activeInHierarchy)
            {
                count++;
            }

            if (count >= collision_count)
            {
                other.transform.Find("Water_Box").gameObject.SetActive(true);
                
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
