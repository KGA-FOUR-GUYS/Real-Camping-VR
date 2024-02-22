using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;

public class XRTwoHandGrabInteractable : XRGrabInteractable
{
    [Space(20)]
    [Header("Custom")]
    public XRCookingToolManager toolManager;

    private IXRSelectInteractor _primaryInteractor = null;
    private IXRSelectInteractable _primaryInteractable = null;
    private IXRSelectInteractor _secondaryInteractor = null;
    private IXRSelectInteractable _secondaryInteractable = null;
    //private Quaternion _primaryAttachPointInitialRotation;
    //private Quaternion _secondaryAttachPointInitialRotation;

    private bool _isOneHandGrabbed => _primaryInteractor != null;
    private bool _isTwoHandGrabbed => _primaryInteractor != null && _secondaryInteractor != null;
    private void Start()
    {
        transform.parent.TryGetComponent(out toolManager);
        Assert.IsNotNull(toolManager, $"Can not find cooking tool manager");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs e)
    {
        base.OnSelectEntered(e);

        //if (!_isOneHandGrabbed)
        //{
        //    attachTransform = toolManager.primaryAttachPoint;
        //    base.OnSelectEntered(e);
        //    toolManager.OnPrimaryGrabEntered(e);

        //    _primaryInteractor = e.interactorObject;
        //    _primaryInteractable = e.interactableObject;
        //}
        //else if (!_isTwoHandGrabbed)
        //{
        //    //useDynamicAttach = true;
        //    secondaryAttachTransform = toolManager.secondaryAttachPoint;
        //    base.OnSelectEntered(e);
        //    toolManager.OnSecondaryGrabEntered(e);

        //    _secondaryInteractor = e.interactorObject;
        //    _secondaryInteractable = e.interactableObject;
        //}
    }

    protected override void OnSelectExited(SelectExitEventArgs e)
    {
        base.OnSelectExited(e);

        //if (_isOneHandGrabbed)
        //{
        //    attachTransform = null;
        //    base.OnSelectExited(e);
        //    toolManager.OnPrimaryGrabExited(e);

        //    _primaryInteractor = null;
        //    _primaryInteractable = null;
        //}
        //else if (_isTwoHandGrabbed)
        //{
        //    //useDynamicAttach = false;
        //    secondaryAttachTransform = null;
        //    base.OnSelectExited(e);
        //    toolManager.OnSecondaryGrabExited(e);

        //    _secondaryInteractor = null;
        //    _secondaryInteractable = null;
        //}
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        //if (_isOneHandGrabbed)
        //{
        //    // Update
        //    if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        //    {
        //        Debug.Log($"On One Handed Grab");
        //    }
        //}
        //else if (_isTwoHandGrabbed)
        //{
        //    // Update
        //    if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        //    {
        //        Debug.Log($"On Two Handed Grab");
        //    }
        //}
    }
}
