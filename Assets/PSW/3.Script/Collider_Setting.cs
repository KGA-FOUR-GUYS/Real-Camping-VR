using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider_Setting : MonoBehaviour
{    
    BoxCollider boxCollider;
    [SerializeField]Vector3 original_position;
    private void OnEnable()
    {
        boxCollider = this.GetComponent<BoxCollider>();
        original_position = boxCollider.center;
    }

    private void Start()
    {
        StartCoroutine(Col_Set_Co());
    }

    private IEnumerator Col_Set_Co()
    {
        while (true)
        {
            boxCollider.center = original_position;
            yield return null;
        }
    }

}
