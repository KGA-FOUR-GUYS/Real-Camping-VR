using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRCookingToolManager : MonoBehaviour
{
    public bool isGrabbed = false;
    [Range(1f,3f)] public float delayAfterGrabExited = 1f;

    [Header("Grabbed Option")]
    public LayerMask grabbedLayer;
    private LayerMask m_initialLayer;

    [Header("Virtual Tool")]
    public GameObject virtualTool;
    private List<Renderer> m_virtualToolRenderers = new List<Renderer>();
    public bool isVirtualToolVisible = true;
    [Range(.01f, 10f)] public float distanceThreshold = .1f;

    private Transform m_virtualToolTransform;

    [Header("Physical Tool")]
    public GameObject physicalTool;
    public float maxSpeed = 5f;

    private Transform m_physicalToolTransform;
    private Rigidbody m_physicalToolRigidbody;

    private Rigidbody m_leftHandPhysicalRigidbody;
    private Rigidbody m_rightHandPhysicalRigidbody;
    private void Awake()
    {
        m_leftHandPhysicalRigidbody = GameObject.FindGameObjectWithTag("LeftHandPhysical").GetComponent<Rigidbody>();
        m_rightHandPhysicalRigidbody = GameObject.FindGameObjectWithTag("RightHandPhysical").GetComponent<Rigidbody>();

        m_virtualToolTransform = virtualTool.transform;
        foreach (var renderer in m_virtualToolTransform.GetComponentsInChildren<Renderer>())
            m_virtualToolRenderers.Add(renderer);

        m_physicalToolTransform = physicalTool.transform;
        m_physicalToolRigidbody = physicalTool.GetComponent<Rigidbody>();

        m_initialLayer = m_physicalToolTransform.gameObject.layer;
    }

    private void FixedUpdate()
    {
        // If tool is grabbed
        // Check distance from interacting physical hand
        // �հ� �Ÿ��� �ʹ� �־����� ��ġ��
    }

    private void Update()
    {
        // If tool is not grabbed
        // Try to match position and rotation (virtual -> physical)
        UpdateVirtualToolPosition();
        UpdateVirtualToolRotation();

        ToggleVirtualHandRenderer();
    }

    private void UpdateVirtualToolPosition()
    {
        if (isGrabbed) return;
        
        m_virtualToolTransform.position = m_physicalToolTransform.position;
    }
    private void UpdateVirtualToolRotation()
    {
        if (isGrabbed) return;

        m_virtualToolTransform.rotation = m_physicalToolTransform.rotation;
    }
    private void ToggleVirtualHandRenderer()
    {
        float distance = Vector3.Distance(m_virtualToolTransform.position, m_physicalToolTransform.position);
        bool isFar = distance >= distanceThreshold;

        foreach (var renderer in m_virtualToolRenderers)
        {
            renderer.enabled = isVirtualToolVisible && isFar;
        }
    }

    // XR Grab Interactable Events
    public void OnGrabEntered(SelectEnterEventArgs e)
    {
        var interactor = e.interactorObject;

        bool isLeftHand = interactor.transform.gameObject.CompareTag("LeftHandInteractor");
        bool isRightHand = interactor.transform.gameObject.CompareTag("RightHandInteractor");

        if (!isLeftHand && !isRightHand) return;

        Rigidbody bodyToConnect = null;
        if (isLeftHand && !isRightHand)
        {
            bodyToConnect = m_leftHandPhysicalRigidbody;
        }
        else if (!isLeftHand && isRightHand)
        {
            bodyToConnect = m_rightHandPhysicalRigidbody;
        }

        if (bodyToConnect == null) return;

        // �߷��� ����/�״� �ϸ� �̻�����... Rigidbody Mass�� �����ϴ� ��� �����
        //m_physicalToolRigidbody.useGravity = false;

        isGrabbed = true;
        TogglePhysicalToolLayer();

        // Physical Tool�� Position, Rotation ����
        Transform handAttachPoint = interactor.transform.GetChild(0);
        m_physicalToolTransform.position = handAttachPoint.position - handAttachPoint.localPosition;
        m_physicalToolTransform.rotation = handAttachPoint.rotation;

        // FixedJoint ����, �����հ� ����
        var jointToHand = m_physicalToolRigidbody.gameObject.AddComponent<FixedJoint>();
        jointToHand.connectedBody = bodyToConnect;
    }
    public void OnGrabExited(SelectExitEventArgs e)
    {
        var interactor = e.interactorObject;

        bool isLeftHand = interactor.transform.gameObject.CompareTag("LeftHandInteractor");
        bool isRightHand = interactor.transform.gameObject.CompareTag("RightHandInteractor");

        if (!isLeftHand && !isRightHand) return;

        // �߷��� ����/�״� �ϸ� �̻�����... Rigidbody Mass�� �����ϴ� ��� �����
        //m_physicalToolRigidbody.useGravity = true;

        isGrabbed = false;
        Invoke(nameof(TogglePhysicalToolLayer), delayAfterGrabExited);

        // FixedJoint ����, �����հ� �и�
        var jointToHand = m_physicalToolRigidbody.gameObject.GetComponent<FixedJoint>();
        Destroy(jointToHand);
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
            collider.gameObject.layer = targetLayerValue;
        }
    }
}
