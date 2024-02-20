using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRTwoHandGrabInteractable : XRGrabInteractable
{
    public XRCookingToolManager toolManager;
    public List<XRSimpleInteractable> secondaryGrabPoints = new List<XRSimpleInteractable>();

    private void Start()
    {
        foreach (var grabPoint in secondaryGrabPoints)
        {
            grabPoint.selectEntered.AddListener(OnSecondHandGrabbed);
            grabPoint.selectExited.AddListener(OnSecondHandReleased);
        }   
    }

    public void OnSecondHandGrabbed(SelectEnterEventArgs e)
    {
        var virtualTool = e.interactableObject.transform.root;
        Debug.Log($"Grabbed : {toolManager.isGrabbed}");
        if (!toolManager.isGrabbed)
        {
            // Tool�� ���� ���� �ʾ�����, SecondaryAttachPoint�� ��Ƶ� PrimaryAttachPoint�� ���� �� ó�� ����
            // OnSelectEntered(e);
        }

        Debug.Log($"OnSecondHandGrabbed [{e.interactableObject.transform.gameObject.name}]");
    }

    public void OnSecondHandReleased(SelectExitEventArgs e)
    {
        var virtualTool = e.interactableObject.transform.root;
        Debug.Log($"Grabbed : {toolManager.isGrabbed}");
        if (!toolManager.isGrabbed)
        {
            // SecondaryAttachPoint�� ���� ��Ȳ�̸�, PrimaryAttachPoint�� ���� �� ó�� ����
            // OnSelectExited(e);
        }

        Debug.Log($"OnSecondHandReleased [{e.interactableObject.transform.gameObject.name}]");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        Debug.Log($"OnPrimaryHandGrabbed");
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        Debug.Log($"OnPrimaryHandReleased");
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
