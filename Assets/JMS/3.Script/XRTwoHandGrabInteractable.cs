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
    public List<XRSimpleInteractable> secondaryGrabPoints = new List<XRSimpleInteractable>();
    public enum RotationType
	{
        None = 0,
        First = 1,
        Second = 2,
    }
    public RotationType _twoHandedRotationType = RotationType.None;

    private IXRSelectInteractor _primaryInteractor = null;
    private IXRSelectInteractable _primaryInteractable = null;
    private IXRSelectInteractor _secondaryInteractor = null;
    private IXRSelectInteractable _secondaryInteractable = null;
    private Quaternion _primaryAttachPointInitialRotation;
    private Quaternion _secondaryAttachPointInitialRotation;

    private bool _isOneHandGrabbed => _primaryInteractor != null && _secondaryInteractor == null;
    private bool _isTwoHandGrabbed => _primaryInteractor != null && _secondaryInteractor != null;
    private void Start()
    {
        foreach (var grabPoint in secondaryGrabPoints)
        {
            grabPoint.selectEntered.AddListener(OnSecondHandGrabbed);
            grabPoint.selectExited.AddListener(OnSecondHandReleased);
        }

        transform.parent.TryGetComponent(out toolManager);
        Assert.IsNotNull(toolManager, $"Can not find cooking tool manager");
    }

    private void FixedUpdate()
    {
        if (_isTwoHandGrabbed)
        {
            MatchPhysicalToolToPrimaryAttachPoint();
        }
    }

    private void MatchPhysicalToolToPrimaryAttachPoint()
    {
        var primaryAttachPoint = _primaryInteractor.GetAttachTransform(_primaryInteractable);
        toolManager.virtualTool.transform.rotation = primaryAttachPoint.rotation;
        toolManager.virtualTool.transform.localRotation *= Quaternion.Euler(-90f, 0f, 0);

        toolManager.physicalTool.transform.rotation = toolManager.virtualTool.transform.rotation;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
	{
        base.ProcessInteractable(updatePhase);

        if (_isTwoHandGrabbed)
		{
            // FixedUpdate
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
            {
                // Compute rotation
                var primaryAttachPoint = _primaryInteractor.GetAttachTransform(_primaryInteractable);
                primaryAttachPoint.rotation = GetTwoHandedRotation();
                // Axe의 AttachPoint의 localRotation의 Inverse만큼 추가 연산 필요
                primaryAttachPoint.localRotation *= Quaternion.Euler(-90f, 0f, 0f);
            }

            // Update
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
			{
                Debug.Log($"On Two Handed Grab");
            }
        }
        else if (_isOneHandGrabbed)
        {
            
        }
	}

    private Quaternion GetTwoHandedRotation()
	{
        var primaryAttachPoint = _primaryInteractor.GetAttachTransform(_primaryInteractable);
        var secondaryAttachPoint = _secondaryInteractor.GetAttachTransform(_secondaryInteractable);

        Quaternion targetRotation = primaryAttachPoint.rotation;

        if (_twoHandedRotationType.Equals(RotationType.None))
		{
            targetRotation = Quaternion.LookRotation(secondaryAttachPoint.position - primaryAttachPoint.position);
        }
        else if (_twoHandedRotationType.Equals(RotationType.First))
		{
            // 오류 발생. 미친듯이 회전...
            targetRotation = Quaternion.LookRotation(secondaryAttachPoint.position - primaryAttachPoint.position, primaryAttachPoint.up);
        }
        else if (_twoHandedRotationType.Equals(RotationType.Second))
		{
            targetRotation = Quaternion.LookRotation(secondaryAttachPoint.position - primaryAttachPoint.position, secondaryAttachPoint.up);
        }

        return targetRotation * Quaternion.Euler(0f, 0f, -90f);
	}

	public void OnSecondHandGrabbed(SelectEnterEventArgs e)
    {
        if (_isOneHandGrabbed)
        {
            toolManager.OnSecondaryGrabEntered(e);

            // Store rotation
            var secondaryAttachPoint = e.interactorObject.GetAttachTransform(e.interactableObject);
            _secondaryAttachPointInitialRotation = secondaryAttachPoint.localRotation;
            _secondaryInteractor = e.interactorObject;
            _secondaryInteractable = e.interactableObject;
        }
    }

    public void OnSecondHandReleased(SelectExitEventArgs e)
    {
        if (_isTwoHandGrabbed)
        {
            toolManager.OnSecondaryGrabExited(e);

            // Reset rotation
            var secondaryAttachPoint = e.interactorObject.GetAttachTransform(e.interactableObject);
            secondaryAttachPoint.localRotation = _secondaryAttachPointInitialRotation;
            _secondaryInteractor = null;
            _secondaryInteractable = null;
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs e)
    {
        base.OnSelectEntered(e);
        toolManager.OnPrimaryGrabEntered(e);

        // Store rotation
        var primaryAttachPoint = e.interactorObject.GetAttachTransform(e.interactableObject);
        _primaryAttachPointInitialRotation = primaryAttachPoint.localRotation;
        _primaryInteractor = e.interactorObject;
        _primaryInteractable = e.interactableObject;
    }

    protected override void OnSelectExited(SelectExitEventArgs e)
    {
        // Release secondary hand when primary hand released
        if (_isTwoHandGrabbed)
		{
            OnSecondHandReleased(e);
        }

        base.OnSelectExited(e);
        toolManager.OnPrimaryGrabExited(e);

        // Reset rotation
        var primaryAttachPoint = _primaryInteractor.GetAttachTransform(_primaryInteractable);
        primaryAttachPoint.localRotation = _primaryAttachPointInitialRotation;
        _primaryInteractor = null;
        _primaryInteractable = null;
    }

    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        var isAlreadySelecting = false;
        foreach (var selectInteractor in interactorsSelecting)
        {
            if (selectInteractor.Equals(interactor))
            {
                isAlreadySelecting = true;
                break;
            }
        }

        var isAlreadyGrabbed = interactorsSelecting.Count > 0 && !isAlreadySelecting;
        return base.IsSelectableBy(interactor) && !isAlreadyGrabbed;
    }
}
