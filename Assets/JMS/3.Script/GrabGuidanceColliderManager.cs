using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(SphereCollider))]
public class GrabGuidanceColliderManager : MonoBehaviour
{
    [Range(1, 30)] public int frameInterval = 5;

    [Range(1f, 10f)] public float maxScale = 5f;
    [Range(10f, 50f)] public float maxDistance = 30f;

    private Transform m_XRHead;

    private void Awake()
    {
        m_XRHead = FindObjectOfType<XRGazeInteractor>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % frameInterval != 0) return;

        float distance = Mathf.Min(maxDistance, Vector3.Distance(m_XRHead.position, transform.position));
        float growRate = distance / maxDistance;
        transform.localScale = Vector3.one * Mathf.Lerp(1f, maxScale, growRate);
    }
}
