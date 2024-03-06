using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyWaterBuoyancy;
using Cooking;

public class FireAndCookingTools : MonoBehaviour
{
    [SerializeField] GameObject Water;
    [SerializeField] GameObject Boil_Area;
    [SerializeField] GameObject Broil_Area;
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            if (Water == null)
            {
                Broil_Area.SetActive(true);
            }
            else
            {
                if (Water.activeInHierarchy)
                {
                    Boil_Area.SetActive(true);
                    Broil_Area.SetActive(false);
                }
                else
                {
                    Boil_Area.SetActive(false);
                    Broil_Area.SetActive(true);
                }
            }
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            if (Water != null)
            {
                Boil_Area.SetActive(false);
            }            
            Broil_Area.SetActive(false);
        }
    }
}
