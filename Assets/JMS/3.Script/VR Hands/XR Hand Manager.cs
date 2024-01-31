using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRHandManager : MonoBehaviour
{
    [Header("Virtual Hand")]
    public GameObject virtualHand;
    [Space(12)]
    public bool isVirtualHandVisible = true;
    [Range(.01f, 10f)] public float distanceThreshold = .1f;

    private Transform m_virtualHandTransform;
    private Renderer m_virtualHandRenderer;

    [Header("Physical Hand")]
    public GameObject physicalHand;
    [Space(12)]
    public bool isTrackingPosition = true;
    public bool isTrackingRotation = true;
    [Space(12)]
    public float maxSpeed = 5f;

    private Transform m_physicalHandTransform;
    private Renderer m_physicalHandRenderer;
    private Rigidbody m_physicalHandRigidbody;

    private void Awake()
    {
        m_virtualHandTransform = virtualHand.transform;
        m_virtualHandRenderer = virtualHand.GetComponent<SkinnedMeshRenderer>();

        m_physicalHandTransform = physicalHand.transform;
        m_physicalHandRenderer = physicalHand.GetComponent<SkinnedMeshRenderer>();
        m_physicalHandRigidbody = physicalHand.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isTrackingPosition) UpdatePhysicalHandPosition();
        if (isTrackingRotation) UpdatePhysicalHandRotation();
    }

    private void LateUpdate()
    {
        if (isVirtualHandVisible) ShowVirtualHand();
    }

    private void UpdatePhysicalHandPosition()
    {
        // Try move to target position
        var desiredVelocity = (m_virtualHandTransform.position - m_physicalHandTransform.position) / Time.fixedDeltaTime;
        if (desiredVelocity.magnitude > maxSpeed)
        {
            var ratio = maxSpeed / desiredVelocity.magnitude;
            desiredVelocity = new Vector3(desiredVelocity.x * ratio, desiredVelocity.y * ratio, desiredVelocity.z * ratio);
        }

        m_physicalHandRigidbody.velocity = desiredVelocity;
    }

    private void UpdatePhysicalHandRotation()
    {
        // Try rotate to target rotation
        Quaternion rotationDiff = m_virtualHandTransform.rotation * Quaternion.Inverse(m_physicalHandTransform.rotation);
        rotationDiff.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        Vector3 rotationDiffInDegree = angleInDegree * rotationAxis;

        m_physicalHandRigidbody.angularVelocity = (rotationDiffInDegree * Mathf.Deg2Rad) / Time.fixedDeltaTime;
    }

    private void ShowVirtualHand()
    {
        float distance = Vector3.Distance(m_virtualHandTransform.position, m_physicalHandTransform.position);
        m_virtualHandRenderer.enabled = distance < distanceThreshold ? false : true;
    }
}
