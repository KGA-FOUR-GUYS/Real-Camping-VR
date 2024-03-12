using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class XRNetworkRigManager : MonoBehaviour
{
    [Header("Network Rig Parts")]
    public Transform NetworkHead;
    public Transform NetworkRightHand;
    public Transform NetworkLeftHand;
    public Animator NetworkRightHandAnimator;
    public Animator NetworkLeftHandAnimator;

    private Transform LocalHead;
    private Transform LocalRightHand;
    private Transform LocalLeftHand;
    private Animator LocalRightHandAnimator;
    private Animator LocalLeftHandAnimator;

    private readonly int GripToHash = Animator.StringToHash("Grip");
    private readonly int TriggerToHash = Animator.StringToHash("Trigger");

    private void Awake()
    {
        // Get required components
        LocalHead = GameObject.FindGameObjectWithTag("HeadPhysical").transform;
        var rightHand = GameObject.FindGameObjectWithTag("RightHandPhysical");
        LocalRightHand = rightHand.transform;
        LocalRightHandAnimator = rightHand.GetComponent<Animator>();

        var leftHand = GameObject.FindGameObjectWithTag("LeftHandPhysical");
        LocalLeftHand = leftHand.transform;
        LocalLeftHandAnimator = leftHand.GetComponent<Animator>();

        // Assertion
        Assert.IsNotNull(NetworkHead);
        Assert.IsNotNull(NetworkRightHand);
        Assert.IsNotNull(NetworkLeftHand);
        Assert.IsNotNull(NetworkRightHandAnimator);
        Assert.IsNotNull(NetworkLeftHandAnimator);

        Assert.IsNotNull(LocalHead);
        Assert.IsNotNull(LocalRightHand);
        Assert.IsNotNull(LocalLeftHand);
        Assert.IsNotNull(LocalRightHandAnimator);
        Assert.IsNotNull(LocalLeftHandAnimator);
    }

    private void Update()
    {
        MatchTransform();
        MatchHandAnimation();
    }

    private void MatchTransform()
    {
        NetworkHead.SetPositionAndRotation(LocalHead.position, LocalHead.rotation);
        NetworkRightHand.SetPositionAndRotation(LocalRightHand.position, LocalRightHand.rotation);
        NetworkLeftHand.SetPositionAndRotation(LocalLeftHand.position, LocalLeftHand.rotation);
    }

    private void MatchHandAnimation()
    {
        NetworkRightHandAnimator.SetFloat(GripToHash, LocalRightHandAnimator.GetFloat(GripToHash));
        NetworkLeftHandAnimator.SetFloat(GripToHash, LocalLeftHandAnimator.GetFloat(GripToHash));

        NetworkRightHandAnimator.SetFloat(TriggerToHash, LocalRightHandAnimator.GetFloat(TriggerToHash));
        NetworkLeftHandAnimator.SetFloat(TriggerToHash, LocalLeftHandAnimator.GetFloat(TriggerToHash));
    }
}
