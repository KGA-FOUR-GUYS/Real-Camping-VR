using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRCookingToolManager : MonoBehaviour
{
    public bool isGrabbed = false;

    public bool isTrackingPosition = true;
    public bool isTrackingRotation = true;

    [Header("Virtual Tool")]
    public GameObject virtualTool;
    public List<Renderer> virtualToolRenderers = new List<Renderer>();
    public bool isVirtualToolVisible = true;
    [Range(.01f, 10f)] public float distanceThreshold = .1f;

    private Transform m_virtualToolTransform;

    [Header("Physical Tool")]
    public GameObject physicalTool;
    public List<Renderer> physicalToolRenders = new List<Renderer>();
    public float maxSpeed = 5f;

    private Transform m_physicalToolTransform;
    private Rigidbody m_physicalToolRigidbody;

    private void Awake()
    {
        m_virtualToolTransform = virtualTool.transform;

        m_physicalToolTransform = physicalTool.transform;
        m_physicalToolRigidbody = physicalTool.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        UpdatePhysicalToolPosition();
        UpdatePhysicalToolRotation();
    }

    private void Update()
    {
        UpdateVirtualToolPosition();
        UpdateVirtualToolRotation();

        ToggleVirtualHandRenderer();
    }

    private void UpdatePhysicalToolPosition()
    {
        if (!isGrabbed || !isTrackingPosition) return;

        // Try to match position (physical -> virtual)
        var desiredVelocity = (m_virtualToolTransform.position - m_physicalToolTransform.position) / Time.fixedDeltaTime;
        if (desiredVelocity.magnitude > maxSpeed)
        {
            var ratio = maxSpeed / desiredVelocity.magnitude;
            desiredVelocity = new Vector3(desiredVelocity.x * ratio, desiredVelocity.y * ratio, desiredVelocity.z * ratio);
        }

        m_physicalToolRigidbody.velocity = desiredVelocity;
    }
    private void UpdatePhysicalToolRotation()
    {
        if (!isGrabbed || !isTrackingRotation) return;

        // Try to match rotation (physical -> virtual)
        Quaternion rotationDiff = m_virtualToolTransform.rotation * Quaternion.Inverse(m_physicalToolTransform.rotation);
        rotationDiff.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        Vector3 rotationDiffInDegree = angleInDegree * rotationAxis;

        m_physicalToolRigidbody.angularVelocity = (rotationDiffInDegree * Mathf.Deg2Rad) / Time.fixedDeltaTime;
    }

    private void UpdateVirtualToolPosition()
    {
        if (isGrabbed || !isTrackingPosition) return;

        // Try to match position (virtual -> physical)
        m_virtualToolTransform.position = m_physicalToolTransform.position;
    }
    private void UpdateVirtualToolRotation()
    {
        if (isGrabbed || !isTrackingRotation) return;

        // Try to match rotation (virtual -> physical)
        m_virtualToolTransform.rotation = m_physicalToolTransform.rotation;
    }

    private void ToggleVirtualHandRenderer()
    {
        float distance = Vector3.Distance(m_virtualToolTransform.position, m_physicalToolTransform.position);
        bool isFar = distance >= distanceThreshold;

        foreach (var renderer in virtualToolRenderers)
        {
            renderer.enabled = isVirtualToolVisible && isFar;
        }
    }
}
