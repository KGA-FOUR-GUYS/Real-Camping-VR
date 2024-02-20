using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;

public class XRTwoHandGrabInteractable : XRGrabInteractable
{
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

	public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
	{
        base.ProcessInteractable(updatePhase);

        // When two-handed grab
        if (_primaryInteractor != null && _secondaryInteractor != null)
		{
            // Compute rotation
            var primaryAttachPoint = _primaryInteractor.GetAttachTransform(_primaryInteractable);
            primaryAttachPoint.rotation = GetTwoHandedRotation();
            // Axe의 AttachPoint의 localRotation의 Inverse만큼 추가 연산 필요
            primaryAttachPoint.localRotation *= Quaternion.Euler(-90f, 0f, 0);

            // Update문에서만 Debug.Log
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
			{
                Debug.Log($"On Two Handed Grab");
            }
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
            targetRotation = Quaternion.LookRotation(secondaryAttachPoint.position - primaryAttachPoint.position, primaryAttachPoint.up);
        }
        else if (_twoHandedRotationType.Equals(RotationType.Second))
		{
            targetRotation = Quaternion.LookRotation(secondaryAttachPoint.position - primaryAttachPoint.position, secondaryAttachPoint.up);
        }

        return targetRotation;
	}

	public void OnSecondHandGrabbed(SelectEnterEventArgs e)
    {
        Debug.Log($"OnSecondHandGrabbed [{e.interactableObject.transform.gameObject.name}]");

        if (!toolManager.isGrabbed)
        {
            // Tool을 아직 잡지 않았으면, SecondaryAttachPoint를 잡아도 PrimaryAttachPoint를 잡은 것 처럼 동작
            // OnSelectEntered(e);
        }

        _secondaryInteractor = e.interactorObject;
        _secondaryInteractable = e.interactableObject;
    }

    public void OnSecondHandReleased(SelectExitEventArgs e)
    {
        Debug.Log($"OnSecondHandReleased [{e.interactableObject.transform.gameObject.name}]");

        if (!toolManager.isGrabbed)
        {
            // SecondaryAttachPoint만 잡은 상황이면, PrimaryAttachPoint를 놓은 것 처럼 동작
            // OnSelectExited(e);
        }

        _secondaryInteractor = null;
        _secondaryInteractable = null;
    }
    
    protected override void OnSelectEntered(SelectEnterEventArgs e)
    {
        base.OnSelectEntered(e);
        Debug.Log($"OnPrimaryHandGrabbed");

        var primaryAttachPoint = e.interactorObject.GetAttachTransform(e.interactableObject);
        _primaryAttachPointInitialRotation = primaryAttachPoint.localRotation;

        _primaryInteractor = e.interactorObject;
        _primaryInteractable = e.interactableObject;
    }

    protected override void OnSelectExited(SelectExitEventArgs e)
    {
        // Release secondary hand, on primary hand released.
        if (_secondaryInteractor != null)
		{
            OnSecondHandReleased(e);
		}

        base.OnSelectExited(e);

        Debug.Log($"OnPrimaryHandReleased");

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
