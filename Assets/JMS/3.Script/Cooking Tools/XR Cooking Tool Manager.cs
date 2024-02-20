using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRCookingToolManager : MonoBehaviour
{
    public bool isGrabbed = false;

    [Header("Grabbed Option")]
    public LayerMask grabbedLayer;
    private LayerMask m_initialLayer;
    [Range(1f, 3f)] public float delayToggleLayerAfterExit = 1f;

    [Header("Virtual Tool")]
    public GameObject virtualTool;
    public Transform grabCollider;
    private Vector3 _grabColliderOriginalLocalPosition;
    private Quaternion _grabColliderOriginalLocalRotation;
    private List<Renderer> m_virtualToolRenderers = new List<Renderer>();
    [Range(.01f, 10f)] public float distanceThreshold = .1f;

    private Transform m_virtualToolTransform;

    [Header("Physical Tool")]
    public GameObject physicalTool;
    public float maxSpeed = 30f;

    private Transform m_physicalToolTransform;
    private Rigidbody m_physicalToolRigidbody;

    private Transform m_leftHandPhysicalTransform;
    private Rigidbody m_leftHandPhysicalRigidbody;

    private Transform m_rightHandPhysicalTransform;
    private Rigidbody m_rightHandPhysicalRigidbody;
    private void Awake()
    {
        m_leftHandPhysicalTransform = GameObject.FindGameObjectWithTag("LeftHandPhysical").transform;
        m_leftHandPhysicalRigidbody = m_leftHandPhysicalTransform.GetComponent<Rigidbody>();

        m_rightHandPhysicalTransform = GameObject.FindGameObjectWithTag("RightHandPhysical").transform;
        m_rightHandPhysicalRigidbody = m_rightHandPhysicalTransform.GetComponent<Rigidbody>();

        m_virtualToolTransform = virtualTool.transform;
        foreach (var renderer in m_virtualToolTransform.GetComponentsInChildren<Renderer>())
            m_virtualToolRenderers.Add(renderer);

        m_physicalToolTransform = physicalTool.transform;
        m_physicalToolRigidbody = physicalTool.GetComponent<Rigidbody>();

        m_initialLayer = m_physicalToolTransform.gameObject.layer;
    }

    private void Start()
    {
        _grabColliderOriginalLocalPosition = grabCollider.localPosition;
        _grabColliderOriginalLocalRotation = grabCollider.localRotation;
    }

    private void FixedUpdate()
    {
        // If tool is grabbed
        // Check distance from interacting physical hand
        // 손과 거리가 너무 멀어지면 놓치기
    }

    private void Update()
    {
        if (!isGrabbed)
        {
            MatchVirtualToolToPhysicalTool();
        }
    }

    private void MatchVirtualToolToPhysicalTool()
    {
        m_virtualToolTransform.rotation = m_physicalToolTransform.rotation;
        m_virtualToolTransform.position = m_physicalToolTransform.position;
    }
    private void ToggleVirtualHandRenderer()
    {
        float distance = Vector3.Distance(m_virtualToolTransform.position, m_physicalToolTransform.position);
        bool isFar = distance >= distanceThreshold;

        foreach (var renderer in m_virtualToolRenderers)
        {
            renderer.enabled = isFar;
        }
    }

    // XR Grab Interactable Events
    public void OnGrabEntered(SelectEnterEventArgs e)
	{
		var interactor = e.interactorObject;

		bool isLeftHand = interactor.transform.gameObject.CompareTag("LeftHandInteractor");
		bool isRightHand = interactor.transform.gameObject.CompareTag("RightHandInteractor");

		if (!isLeftHand && !isRightHand) return;

        Rigidbody bodyToConnect = isLeftHand ? m_leftHandPhysicalRigidbody : m_rightHandPhysicalRigidbody;

		TogglePhysicalToolLayer();
		MatchPhysicalToolToPhysicalHand(isLeftHand);

		// FixedJoint 생성, 물리손과 연결
		var jointToHand = m_physicalToolRigidbody.gameObject.AddComponent<FixedJoint>();
		jointToHand.connectedBody = bodyToConnect;

        isGrabbed = true;
    }

	private void MatchPhysicalToolToPhysicalHand(bool isLeftHand)
	{
        Transform hand = isLeftHand ? m_leftHandPhysicalTransform : m_rightHandPhysicalTransform;
        Transform handAttachPoint = hand.GetChild(0);

        Transform tool = m_physicalToolTransform;
        Transform toolAttachPoint = tool.GetChild(0);

        // toolAttachPoint에 맞게 회전
        Quaternion rotationOffset = Quaternion.Inverse(tool.rotation) * toolAttachPoint.rotation;
        tool.rotation = handAttachPoint.rotation * Quaternion.Inverse(rotationOffset);

        // toolAttachPoint에 맞게 위치
        Vector3 positionOffset = tool.position - toolAttachPoint.position;
        tool.position = handAttachPoint.position + positionOffset;

        // Grab collider 위치 변경
        grabCollider.position = toolAttachPoint.position;
        grabCollider.rotation = toolAttachPoint.rotation;
        grabCollider.SetParent(toolAttachPoint);
    }

    public void OnGrabExited(SelectExitEventArgs e)
    {
        var interactor = e.interactorObject;

        bool isLeftHand = interactor.transform.gameObject.CompareTag("LeftHandInteractor");
        bool isRightHand = interactor.transform.gameObject.CompareTag("RightHandInteractor");

        if (!isLeftHand && !isRightHand) return;

        Invoke(nameof(TogglePhysicalToolLayer), delayToggleLayerAfterExit);

        // FixedJoint 삭제, 물리손과 분리
        var jointToHand = m_physicalToolRigidbody.gameObject.GetComponent<FixedJoint>();
        Destroy(jointToHand);

        // Grab collider 위치 복원
        grabCollider.SetParent(m_virtualToolTransform);
        grabCollider.localPosition = _grabColliderOriginalLocalPosition;
        grabCollider.localRotation = _grabColliderOriginalLocalRotation;

        isGrabbed = false;
    }
    private void TogglePhysicalToolLayer()
    {
        int currentLayerValue = m_physicalToolTransform.gameObject.layer;

        LayerMask targetLayerMask = isGrabbed ? grabbedLayer : m_initialLayer;
        int targetLayerValue = targetLayerMask == 0
                                ? 0
                                : targetLayerMask.value < 0
                                    ? (int)Mathf.Log(targetLayerMask.value - 1, 2f)
                                    : (int)Mathf.Log(targetLayerMask.value, 2f);

        if (currentLayerValue == targetLayerValue) return;

        m_physicalToolTransform.gameObject.layer = targetLayerValue;

        // Change layer of children
        var colliders = m_physicalToolTransform.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            if (collider.gameObject.layer != LayerMask.NameToLayer("Default")) continue;
            collider.gameObject.layer = targetLayerValue;
        }
    }
}
