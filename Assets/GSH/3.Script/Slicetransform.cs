using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicetransform : MonoBehaviour
{
    public Transform topParent;
    public ObjectSpawner spawner;
    private void Awake()
    {
        topParent = GetTopParent(gameObject);
        spawner = topParent.gameObject.GetComponent<ObjectSpawner>();
    }

    public Transform GetTopParent(GameObject childObject)
    {
        Transform currentTransform = childObject.transform;

        while (currentTransform.parent != null)
        {
            currentTransform = currentTransform.parent;
        }

        return currentTransform;
    }
}
