using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyWaterBuoyancy;

[RequireComponent(typeof(FloatingObject))]
public class DensityControl : MonoBehaviour
{
    private FloatingObject _floatingObj;
    [SerializeField] float deeping_time;
    [SerializeField] float density_max;
    [SerializeField] float Crescendo_con;
    [SerializeField] bool triggerOn = false;
    IEnumerator current_co;
    float original_density;

    private void Awake()
    {
        TryGetComponent(out _floatingObj);
        current_co = Density_Crescendo_Co();
        original_density = _floatingObj.density;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(WaterVolume.TAG) && !triggerOn)
        {
            triggerOn = true;
            StopAllCoroutines();
            StartCoroutine(current_co);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ladle"))
        {
            StopAllCoroutines();
            _floatingObj.density = original_density;
            StartCoroutine(current_co);
            triggerOn = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(WaterVolume.TAG))
        {
            triggerOn = false;
            StopAllCoroutines();
            _floatingObj.density = original_density;
        }
    }

    private IEnumerator Density_Crescendo_Co()
    {
        yield return new WaitForSeconds(deeping_time);
        while (_floatingObj.density < density_max)
        {
            _floatingObj.density += Crescendo_con * Time.deltaTime;
            yield return null;
        }
        triggerOn = false;
    }
}
