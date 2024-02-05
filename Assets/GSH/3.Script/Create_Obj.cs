using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Create_Obj : MonoBehaviour
{
    public GameObject CreatePrefab;

    private void OnEnable()
    {
        GetComponent<XRGrabInteractable>().selectEntered.AddListener(OnSelectEntered);
    }
    private void OnDisable()
    {
        GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        SpawnObject();
    }

    private void SpawnObject()
    {
        if (CreatePrefab != null)
        {
            Instantiate(CreatePrefab, transform.position, transform.rotation);
        }
        else
        {
            Debug.LogError("Object to spawn is not assigned!");
        }
    }
}
