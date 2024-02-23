using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;

public class XRCookingToolManager : MonoBehaviour
{
    public enum CookingToolType
    {
        OneHand = 1,
        TwoHand = 2,
    }

    public bool isPrimaryGrabbed = false;
    public bool isSecondaryGrabbed = false;

    [Header("Grabbed Option")]
    public CookingToolType toolType = CookingToolType.OneHand;
    public LayerMask grabbedLayer;
    [Range(1f, 3f)] public float delayToggleLayerAfterExit = 1f;

    [Header("Virtual Tool")]
    public bool isVirtualHandVisible = false;
    public GameObject virtualTool;
    public Collider grabCollider;
    public Renderer virtualToolRenderer;
    [Range(.01f, 10f)] public float distanceThreshold = .1f;

    [Header("Physical Tool")]
    public GameObject physicalTool;
    public Transform primaryAttachPoint;
    public float maxSpeed = 30f;
    [Range(0f, 1f)] public float connectedBodyMassScale = 1f;

    private Transform _virtualToolTransform;

    private Transform _physicalToolTransform;
    private Rigidbody _physicalToolRigidbody;

    private LayerMask _initialLayer;

    private Transform _leftHandPhysicalTransform;
    private Rigidbody _leftHandPhysicalRigidbody;
    private Transform _rightHandPhysicalTransform;
    private Rigidbody _rightHandPhysicalRigidbody;

    private void Start()
    {
        _virtualToolTransform = virtualTool.transform;

        _physicalToolTransform = physicalTool.transform;
        _physicalToolRigidbody = physicalTool.GetComponent<Rigidbody>();

        _initialLayer = _physicalToolTransform.gameObject.layer;

        _leftHandPhysicalTransform = GameObject.FindGameObjectWithTag("LeftHandPhysical").transform;
        _leftHandPhysicalRigidbody = _leftHandPhysicalTransform.GetComponent<Rigidbody>();
        _rightHandPhysicalTransform = GameObject.FindGameObjectWithTag("RightHandPhysical").transform;
        _rightHandPhysicalRigidbody = _rightHandPhysicalTransform.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!isPrimaryGrabbed)
        {
            MatchVirtualToolToPhysicalTool();
        }
        else if (isSecondaryGrabbed)
        {
            MatchPhysicalToolToVirtualTool();
        }
        // If tool is grabbed
        // Check distance from interacting physical hand
        // 손과 거리가 너무 멀어지면 놓치기
    }

    private void Update()
    {
        ToggleVirtualToolRenderer();
    }

    private void MatchVirtualToolToPhysicalTool()
    {
        _virtualToolTransform.rotation = _physicalToolTransform.rotation;
        _virtualToolTransform.position = _physicalToolTransform.position;
    }

    private void MatchPhysicalToolToVirtualTool()
    {
        UpdatePhysicalToolRotation();
        UpdatePhysicalToolPosition();
    }

    private void UpdatePhysicalToolRotation()
    {
        // Try to match rotation (physical -> virtual)
        Quaternion rotationDiff = _virtualToolTransform.rotation * Quaternion.Inverse(_physicalToolTransform.rotation);
        rotationDiff.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        Vector3 rotationDiffInDegree = angleInDegree * rotationAxis;

        _physicalToolRigidbody.angularVelocity = (rotationDiffInDegree * Mathf.Deg2Rad) / Time.fixedDeltaTime;
    }

    private void UpdatePhysicalToolPosition()
    {
        // Try to match position (physical -> virtual)
        var desiredVelocity = (_virtualToolTransform.position - _physicalToolTransform.position) / Time.fixedDeltaTime;
        if (desiredVelocity.magnitude > maxSpeed)
        {
            var ratio = maxSpeed / desiredVelocity.magnitude;
            desiredVelocity = new Vector3(desiredVelocity.x * ratio, desiredVelocity.y * ratio, desiredVelocity.z * ratio);
        }

        _physicalToolRigidbody.velocity = desiredVelocity;
    }

    private IXRSelectInteractor _primaryInteractor = null;
    private IXRSelectInteractor _secondaryInteractor = null;
    // XR Grab Interactable Events
    public void OnGrabEntered(SelectEnterEventArgs e)
	{
		bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");
		bool isRightHand = e.interactorObject.transform.gameObject.CompareTag("RightHandInteractor");
		if (!isLeftHand && !isRightHand) return;

        if (!isPrimaryGrabbed)
        {
            _primaryInteractor = e.interactorObject;
            isPrimaryGrabbed = true;
            TogglePhysicalToolLayer();
            AttachPrimaryPointToHand(e);
        }
        else if (!isSecondaryGrabbed)
        {
            _secondaryInteractor = e.interactorObject;
            isSecondaryGrabbed = true;
            AttachSecondaryPointToHand(e);
        }
    }

    // XR Grab Interactable Events
    public void OnGrabExited(SelectExitEventArgs e)
    {
        if (isPrimaryGrabbed && !isSecondaryGrabbed)
        {
            _primaryInteractor = null;
            isPrimaryGrabbed = false;
            Invoke(nameof(TogglePhysicalToolLayer), delayToggleLayerAfterExit);
            DetachPrimaryPointFromHand(e);
        }
        else if (isPrimaryGrabbed && isSecondaryGrabbed)
        {
            _secondaryInteractor = null;
            isSecondaryGrabbed = false;
            DetachSecondaryPointFromHand(e);
        }
    }

    private void TogglePhysicalToolLayer()
    {
        int currentLayerValue = _physicalToolTransform.gameObject.layer;

        LayerMask targetLayerMask = isPrimaryGrabbed ? grabbedLayer : _initialLayer;
        int targetLayerValue = targetLayerMask == 0
                                ? 0
                                : targetLayerMask.value < 0
                                    ? (int)Mathf.Log(targetLayerMask.value - 1, 2f)
                                    : (int)Mathf.Log(targetLayerMask.value, 2f);

        if (currentLayerValue == targetLayerValue) return;

        _physicalToolTransform.gameObject.layer = targetLayerValue;

        // Change layer of children
        var colliders = _physicalToolTransform.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            if (collider.gameObject.layer != LayerMask.NameToLayer("Default")) continue;
            collider.gameObject.layer = targetLayerValue;
        }
    }

    private Vector3 _primaryColliderLocalPosition;
    private Quaternion _primaryColliderLocalRotation;
    private FixedJoint _primaryJointToHand;
    private void AttachPrimaryPointToHand(SelectEnterEventArgs e)
	{
        bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");

        Transform hand = isLeftHand ? _leftHandPhysicalTransform : _rightHandPhysicalTransform;
        Transform handAttachPoint = hand.GetChild(0);

        // Physical Tool 회전
        Quaternion rotationOffset = Quaternion.Inverse(_physicalToolTransform.rotation) * primaryAttachPoint.rotation;
        _physicalToolTransform.rotation = handAttachPoint.rotation * Quaternion.Inverse(rotationOffset);

        // Physical Tool 이동
        Vector3 positionOffset = _physicalToolTransform.position - primaryAttachPoint.position;
        _physicalToolTransform.position = handAttachPoint.position + positionOffset;

        // Priamry Grab collider 보정
        _primaryColliderLocalPosition = grabCollider.transform.localPosition;
        _primaryColliderLocalRotation = grabCollider.transform.localRotation;
        grabCollider.transform.SetParent(primaryAttachPoint);
        grabCollider.transform.rotation = primaryAttachPoint.rotation;
        // grabCollider.transform.position = primaryAttachPoint.position;
        // grabCollider의 localPosition을 attachPoint의 회전에 맞게 변환해주어 적용하면 완벽한 위치에 grabCollider를 둘 수 있다...

        // Hand를 기준으로 Physical Tool을 FixedJoint로 연결
        Rigidbody bodyToConnect = isLeftHand ? _leftHandPhysicalRigidbody : _rightHandPhysicalRigidbody;
        _primaryJointToHand = _physicalToolRigidbody.gameObject.AddComponent<FixedJoint>();
        _primaryJointToHand.connectedBody = bodyToConnect;
        _primaryJointToHand.connectedMassScale = connectedBodyMassScale;

        string context = isLeftHand ? "Left Hand" : "Right Hand";
        Debug.Log($"Primary Hand Grabbed: {context}");
    }

    private void DetachPrimaryPointFromHand(SelectExitEventArgs e)
    {
        Destroy(_primaryJointToHand);

        // Primary Grab collider 원복
        grabCollider.transform.SetParent(_virtualToolTransform);
        grabCollider.transform.localPosition = _primaryColliderLocalPosition;
        grabCollider.transform.localRotation = _primaryColliderLocalRotation;

        bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");
        string context = isLeftHand ? "Left Hand" : "Right Hand";
        Debug.Log($"Primary Hand Released: {context}");
    }


    private void AttachSecondaryPointToHand(SelectEnterEventArgs e)
    {
        Debug.Log("AttachSecondaryPointToHand");
    }

    private void DetachSecondaryPointFromHand(SelectExitEventArgs e)
    {
        Debug.Log("DetachSecondaryPointFromHand");
    }

    private void ToggleVirtualToolRenderer()
    {
        float distance = Vector3.Distance(_virtualToolTransform.position, _physicalToolTransform.position);
        bool isFar = distance >= distanceThreshold;

        virtualToolRenderer.enabled = isVirtualHandVisible && isFar;
    }
}
