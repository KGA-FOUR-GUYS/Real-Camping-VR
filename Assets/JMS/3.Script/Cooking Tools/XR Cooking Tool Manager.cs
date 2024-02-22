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
    public LayerMask grabbedLayer;
    [Range(1f, 3f)] public float delayToggleLayerAfterExit = 1f;

    [Header("Virtual Tool")]
    public CookingToolType toolType = CookingToolType.OneHand;
    public GameObject virtualTool;
    public Collider primaryGrabCollider;
    public List<Collider> secondaryGrabColliders = new List<Collider>();
    
    [Header("Physical Tool")]
    public GameObject physicalTool;
    public Transform primaryAttachPoint;
    public List<Transform> secondaryAttachPoints = new List<Transform>();

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

        if (toolType == CookingToolType.TwoHand)
        {
            Assert.IsTrue(secondaryGrabColliders.Count == secondaryAttachPoints.Count
                        , $"secondaryGrabColliders.Count and secondaryAttachPoints.Count have to be same");
        }
    }

    private void FixedUpdate()
    {
        if (!isPrimaryGrabbed)
        {
            MatchVirtualToolToPhysicalTool();
        }

        // If tool is grabbed
        // Check distance from interacting physical hand
        // 손과 거리가 너무 멀어지면 놓치기
    }

    private void MatchVirtualToolToPhysicalTool()
    {
        _virtualToolTransform.rotation = _physicalToolTransform.rotation;
        _virtualToolTransform.position = _physicalToolTransform.position;
    }

    // XR Grab Interactable Events
    public void OnPrimaryGrabEntered(SelectEnterEventArgs e)
	{
		bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");
		bool isRightHand = e.interactorObject.transform.gameObject.CompareTag("RightHandInteractor");
		if (!isLeftHand && !isRightHand) return;

        isPrimaryGrabbed = true;

		TogglePhysicalToolLayer();
		AttachPrimaryPointToHand(e);
    }
    public void OnPrimaryGrabExited(SelectExitEventArgs e)
    {
        isPrimaryGrabbed = false;

        Invoke(nameof(TogglePhysicalToolLayer), delayToggleLayerAfterExit);
        DetachPrimaryPointFromHand(e);
    }
    public void OnSecondaryGrabEntered(SelectEnterEventArgs e)
    {
        bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");
        bool isRightHand = e.interactorObject.transform.gameObject.CompareTag("RightHandInteractor");
        if (!isLeftHand && !isRightHand) return;

        isSecondaryGrabbed = true;

        //AttachSecondaryPointToHand(e);
    }
    public void OnSecondaryGrabExited(SelectExitEventArgs e)
    {
        bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");
        bool isRightHand = e.interactorObject.transform.gameObject.CompareTag("RightHandInteractor");
        if (!isLeftHand && !isRightHand) return;

        isSecondaryGrabbed = false;

        //DetachSecondaryPointFromHand(e);
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

    private Collider _primaryGrabCollider;
    private Transform _primaryAttachPoint;
    private Vector3 _primaryColliderLocalPos;
    private Quaternion _primaryColliderLocalRot;
    private FixedJoint _primaryJointToHand;
    private void AttachPrimaryPointToHand(SelectEnterEventArgs e)
	{
        // e로 받아와야 일반화 가능...
        //var virtualHandAttachPoint = e.interactorObject.GetAttachTransform(e.interactableObject);
        //var virtualHandCollider = virtualHandAttachPoint.parent.GetComponent<Collider>();
        //_debugAttachPoint = virtualHandAttachPoint;
        //Collider[] colliders = Physics.OverlapSphere(virtualHandAttachPoint.position, _debugRadius, 1 << LayerMask.NameToLayer("DirectGrab"));
        //foreach (var collider in colliders)
        //{
        //    if (collider != virtualHandCollider)
        //    {
        //        _primaryGrabCollider = collider;
        //        break;
        //    }
        //}
        //_primaryAttachPoint = e.interactableObject.GetAttachTransform(e.interactorObject);
        _primaryGrabCollider = primaryGrabCollider;
        _primaryAttachPoint = primaryAttachPoint;

        bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");

        Transform hand = isLeftHand ? _leftHandPhysicalTransform : _rightHandPhysicalTransform;
        Transform handAttachPoint = hand.GetChild(0);

        // Physical Tool 회전
        Quaternion rotationOffset = Quaternion.Inverse(_physicalToolTransform.rotation) * primaryAttachPoint.rotation;
        _physicalToolTransform.rotation = handAttachPoint.rotation * Quaternion.Inverse(rotationOffset);

        // Physical Tool 이동
        Vector3 positionOffset = _physicalToolTransform.position - primaryAttachPoint.position;
        _physicalToolTransform.position = handAttachPoint.position + positionOffset;

        // Priamry Grab collider 저장
        _primaryColliderLocalPos = _primaryGrabCollider.transform.localPosition;
        _primaryColliderLocalRot = _primaryGrabCollider.transform.localRotation;
        // Priamry Grab collider 이동
        _primaryGrabCollider.transform.position = _primaryAttachPoint.position;
        _primaryGrabCollider.transform.rotation = _primaryAttachPoint.rotation;
        _primaryGrabCollider.transform.SetParent(_primaryAttachPoint);

        if (toolType == CookingToolType.TwoHand)
        {
            // Secondary Grab collider 이동
            for (int i = 0; i < secondaryGrabColliders.Count; i++)
            {
                var grabCollider = secondaryGrabColliders[i];
                var attachPoint = secondaryAttachPoints[i];

                grabCollider.transform.position = attachPoint.position;
                grabCollider.transform.rotation = attachPoint.rotation;
                grabCollider.transform.SetParent(attachPoint);
            }
        }

        // Hand를 기준으로 Physical Tool을 FixedJoint로 연결
        Rigidbody bodyToConnect = isLeftHand ? _leftHandPhysicalRigidbody : _rightHandPhysicalRigidbody;
        _primaryJointToHand = _physicalToolRigidbody.gameObject.AddComponent<FixedJoint>();
        _primaryJointToHand.connectedBody = bodyToConnect;

        string context = isLeftHand ? "Left Hand" : "Right Hand";
        Debug.Log($"Primary Hand Grabbed: {context}");
    }

    private void DetachPrimaryPointFromHand(SelectExitEventArgs e)
    {
        _debugAttachPoint = null;

        Destroy(_primaryJointToHand);

        // Primary Grab collider 복원
        _primaryGrabCollider.transform.SetParent(_virtualToolTransform);
        _primaryGrabCollider.transform.localPosition = _primaryColliderLocalPos;
        _primaryGrabCollider.transform.localRotation = _primaryColliderLocalRot;

        bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");
        string context = isLeftHand ? "Left Hand" : "Right Hand";
        Debug.Log($"Primary Hand Released: {context}");
    }


    private Collider _secondaryGrabCollider;
    private Transform _secondaryAttachPoint;
    private Vector3 _secondaryColliderOriginPosition;
    private Quaternion _secondaryColliderOriginRotation;
    private FixedJoint _secondaryJointToHand;
    private void AttachSecondaryPointToHand(SelectEnterEventArgs e)
    {
        bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");

        // e로 받아와야 동작 가능...
        _secondaryGrabCollider = e.interactableObject.GetAttachTransform(e.interactorObject).GetComponent<Collider>();
        _secondaryAttachPoint = e.interactableObject.GetAttachTransform(e.interactorObject);

        // Secondary Grab collider 저장
        _secondaryColliderOriginPosition = _secondaryGrabCollider.transform.localPosition;
        _secondaryColliderOriginRotation = _secondaryGrabCollider.transform.localRotation;
        // Priamry Grab collider 이동
        _secondaryGrabCollider.transform.position = _secondaryAttachPoint.position;
        _secondaryGrabCollider.transform.rotation = _secondaryAttachPoint.rotation;
        _secondaryGrabCollider.transform.SetParent(_secondaryAttachPoint);

        // Hand를 기준으로 Physical Tool을 FixedJoint로 연결
        Rigidbody bodyToConnect = isLeftHand ? _leftHandPhysicalRigidbody : _rightHandPhysicalRigidbody;
        _secondaryJointToHand = _physicalToolRigidbody.gameObject.AddComponent<FixedJoint>();
        _secondaryJointToHand.connectedBody = bodyToConnect;

        string context = isLeftHand ? "Left Hand" : "Right Hand";
        Debug.Log($"Secondary Hand Grabbed: {context}");
    }

    private void DetachSecondaryPointFromHand(SelectExitEventArgs e)
    {
        Destroy(_secondaryJointToHand);

        // Secondary Grab collider 위치 복원
        _secondaryGrabCollider.transform.SetParent(_virtualToolTransform);
        _secondaryGrabCollider.transform.localPosition = _secondaryColliderOriginPosition;
        _secondaryGrabCollider.transform.localRotation = _secondaryColliderOriginRotation;

        bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");
        string context = isLeftHand ? "Left Hand" : "Right Hand";
        Debug.Log($"Primary Hand Released: {context}");
    }

    public float _debugRadius = .2f;
    private Transform _debugAttachPoint;
    private void OnDrawGizmos()
    {
        if (_debugAttachPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_debugAttachPoint.position, _debugRadius);
        }
    }
}
