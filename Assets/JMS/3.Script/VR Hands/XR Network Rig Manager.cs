using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Mirror;

public class XRNetworkRigManager : NetworkBehaviour
{
    public XRLocalRigManager localRigManager;

    [Header("Local Rig Parts")]
    public Transform LocalHead;
    public Transform LocalRightHand;
    public Transform LocalLeftHand;
    public Animator LocalRightHandAnimator;
    public Animator LocalLeftHandAnimator;
    private Rigidbody LocalRightHandRigidbody;
    private Rigidbody LocalLeftHandRigidbody;

    [Header("Network Rig Parts")]
    public Transform NetworkHead;
    public Transform NetworkRightHand;
    public Transform NetworkLeftHand;
    private Animator NetworkRightHandAnimator;
    private Animator NetworkLeftHandAnimator;
    private Rigidbody NetworkRightHandRigidbody;
    private Rigidbody NetworkLeftHandRigidbody;

    private void Awake()
    {
        NetworkRightHandAnimator = NetworkRightHand.GetComponent<Animator>();
        NetworkRightHandRigidbody = NetworkRightHand.GetComponent<Rigidbody>();

        NetworkLeftHandAnimator = NetworkLeftHand.GetComponent<Animator>();
        NetworkLeftHandRigidbody = NetworkLeftHand.GetComponent<Rigidbody>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        CacheRequiredComponents();
        AssertRequiredComponents();

        localRigManager.playerNetId = GetComponent<NetworkIdentity>().netId;
    }
    
    private void CacheRequiredComponents()
    {
        localRigManager = FindObjectOfType<XRLocalRigManager>();

        LocalHead = localRigManager.localHead;

        var rightHand = localRigManager.localRightHand;
        LocalRightHand = rightHand.transform;
        LocalRightHandAnimator = rightHand.GetComponent<Animator>();
        LocalRightHandRigidbody = rightHand.GetComponent<Rigidbody>();

        var leftHand = localRigManager.localLeftHand;
        LocalLeftHand = leftHand.transform;
        LocalLeftHandAnimator = leftHand.GetComponent<Animator>();
        LocalLeftHandRigidbody = leftHand.GetComponent<Rigidbody>();
    }
    
    private void AssertRequiredComponents()
    {
        Assert.IsNotNull(NetworkHead);
        Assert.IsNotNull(NetworkRightHand);
        Assert.IsNotNull(NetworkRightHandAnimator);
        Assert.IsNotNull(NetworkRightHandRigidbody);
        Assert.IsNotNull(NetworkLeftHand);
        Assert.IsNotNull(NetworkLeftHandAnimator);
        Assert.IsNotNull(NetworkLeftHandRigidbody);

        Assert.IsNotNull(LocalHead);
        Assert.IsNotNull(LocalRightHand);
        Assert.IsNotNull(LocalRightHandAnimator);
        Assert.IsNotNull(LocalRightHandRigidbody);
        Assert.IsNotNull(LocalLeftHand);
        Assert.IsNotNull(LocalLeftHandAnimator);
        Assert.IsNotNull(LocalLeftHandRigidbody);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        MatchNetworkHandRigidbody();
    }

    private void MatchNetworkHandRigidbody()
    {
        NetworkRightHandRigidbody.position = LocalRightHandRigidbody.position;
        NetworkRightHandRigidbody.rotation = LocalRightHandRigidbody.rotation;
        NetworkRightHandRigidbody.velocity = LocalRightHandRigidbody.velocity;
        NetworkRightHandRigidbody.angularVelocity = LocalRightHandRigidbody.angularVelocity;
        NetworkRightHandRigidbody.isKinematic = LocalRightHandRigidbody.isKinematic;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        MatchNetworkRigTransform();
        MatchNetworkHandAnimation();
    }

    private void MatchNetworkRigTransform()
    {
        NetworkHead.SetPositionAndRotation(LocalHead.position, LocalHead.rotation);
        NetworkRightHand.SetPositionAndRotation(LocalRightHand.position, LocalRightHand.rotation);
        NetworkLeftHand.SetPositionAndRotation(LocalLeftHand.position, LocalLeftHand.rotation);
    }

    private readonly int GripToHash = Animator.StringToHash("Grip");
    private readonly int TriggerToHash = Animator.StringToHash("Trigger");
    private void MatchNetworkHandAnimation()
    {
        NetworkRightHandAnimator.SetFloat(GripToHash, LocalRightHandAnimator.GetFloat(GripToHash));
        NetworkLeftHandAnimator.SetFloat(GripToHash, LocalLeftHandAnimator.GetFloat(GripToHash));

        NetworkRightHandAnimator.SetFloat(TriggerToHash, LocalRightHandAnimator.GetFloat(TriggerToHash));
        NetworkLeftHandAnimator.SetFloat(TriggerToHash, LocalLeftHandAnimator.GetFloat(TriggerToHash));
    }
}
