using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRHandManager : MonoBehaviour
{
    [Header("Virtual Hand")]
    public GameObject virtualHand;
    public bool isVirtualHandVisible = false;
    [Range(.01f, 10f)] public float distanceThreshold = .1f;

    private Transform m_virtualHandTransform;
    private Renderer m_virtualHandRenderer;

    [Header("Physical Hand")]
    public GameObject physicalHand;
    public bool isTrackingPosition = true;
    public bool isTrackingRotation = true;
    public float maxSpeed = 30f;

    private Transform m_physicalHandTransform;
    private Rigidbody m_physicalHandRigidbody;

    private void Awake()
    {
        m_virtualHandTransform = virtualHand.transform;
        m_virtualHandRenderer = virtualHand.GetComponent<SkinnedMeshRenderer>();

        m_physicalHandTransform = physicalHand.transform;
        m_physicalHandRigidbody = physicalHand.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        UpdatePhysicalHandRotation();
        UpdatePhysicalHandPosition();
    }

    private void Update()
    {
        ToggleVirtualHandRenderer();
    }
    private void UpdatePhysicalHandRotation()
    {
        if (!isTrackingRotation) return;

        // Try to match rotation (physical -> virtual)
        Quaternion rotationDiff = m_virtualHandTransform.rotation * Quaternion.Inverse(m_physicalHandTransform.rotation);
        rotationDiff.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        Vector3 rotationDiffInDegree = angleInDegree * rotationAxis;

        m_physicalHandRigidbody.angularVelocity = (rotationDiffInDegree * Mathf.Deg2Rad) / Time.fixedDeltaTime;
    }

    private void UpdatePhysicalHandPosition()
    {
        if (!isTrackingPosition) return;

        // Try to match position (physical -> virtual)
        var desiredVelocity = (m_virtualHandTransform.position - m_physicalHandTransform.position) / Time.fixedDeltaTime;
        if (desiredVelocity.magnitude > maxSpeed)
        {
            var ratio = maxSpeed / desiredVelocity.magnitude;
            desiredVelocity = new Vector3(desiredVelocity.x * ratio, desiredVelocity.y * ratio, desiredVelocity.z * ratio);
        }

        m_physicalHandRigidbody.velocity = desiredVelocity;
    }

    private void ToggleVirtualHandRenderer()
    {
        float distance = Vector3.Distance(m_virtualHandTransform.position, m_physicalHandTransform.position);
        bool isFar = distance >= distanceThreshold;

        m_virtualHandRenderer.enabled = isVirtualHandVisible && isFar;
    }
}
