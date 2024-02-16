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
    public float offsetHandUp = 0f;
    public float offsetHandRight = 0f;

    [Header("Virtual Tool")]
    public GameObject virtualTool;
    private List<Renderer> m_virtualToolRenderers = new List<Renderer>();
    public bool isVirtualToolVisible = true;
    [Range(.01f, 10f)] public float distanceThreshold = .1f;

    private Transform m_virtualToolTransform;

    [Header("Physical Tool")]
    public GameObject physicalTool;
    public float maxSpeed = 30f;

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

    private void Start()
    {
        foreach (var renderer in m_virtualToolRenderers)
        {
            renderer.enabled = isVirtualToolVisible;
        }
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
            // Try to match position and rotation (virtual -> physical)
            UpdateVirtualToolPosition();
            UpdateVirtualToolRotation();
        }

        if (isVirtualToolVisible)
        {
            ToggleVirtualHandRenderer();
        }
    }

    private void UpdateVirtualToolPosition()
    {
        m_virtualToolTransform.position = m_physicalToolTransform.position;
    }
    private void UpdateVirtualToolRotation()
    {
        m_virtualToolTransform.rotation = m_physicalToolTransform.rotation;
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

        // 중력을 껐다/켰다 하면 이상해짐... Rigidbody Mass를 조정하는 방식 사용중
        //m_physicalToolRigidbody.useGravity = false;

        isGrabbed = true;
        TogglePhysicalToolLayer();

        // Physical Tool의 Position, Rotation 변경
        Transform physicalHand = isLeftHand
                                 ? m_leftHandPhysicalRigidbody.transform
                                 : m_rightHandPhysicalRigidbody.transform;
        Transform physicalHandAttachPoint = physicalHand.GetChild(0);

        int direction = isLeftHand ? -1 : 1;
        m_physicalToolTransform.position = physicalHandAttachPoint.position
                                            + physicalHand.up * offsetHandUp
                                            + physicalHand.right * offsetHandRight * direction;

        m_physicalToolTransform.rotation = physicalHandAttachPoint.rotation;

        // FixedJoint 생성, 물리손과 연결
        var jointToHand = m_physicalToolRigidbody.gameObject.AddComponent<FixedJoint>();
        jointToHand.connectedBody = bodyToConnect;
    }
    public void OnGrabExited(SelectExitEventArgs e)
    {
        var interactor = e.interactorObject;

        bool isLeftHand = interactor.transform.gameObject.CompareTag("LeftHandInteractor");
        bool isRightHand = interactor.transform.gameObject.CompareTag("RightHandInteractor");

        if (!isLeftHand && !isRightHand) return;

        // 중력을 껐다/켰다 하면 이상해짐... Rigidbody Mass를 조정하는 방식 사용중
        //m_physicalToolRigidbody.useGravity = true;

        isGrabbed = false;
        Invoke(nameof(TogglePhysicalToolLayer), delayToggleLayerAfterExit);

        // FixedJoint 삭제, 물리손과 분리
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
            if (collider.gameObject.layer != LayerMask.NameToLayer("Default")) continue;
            collider.gameObject.layer = targetLayerValue;
        }
    }
}
