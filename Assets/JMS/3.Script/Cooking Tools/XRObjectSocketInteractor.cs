using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRObjectSocketInteractor : XRSocketInteractor
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override bool CanHover(IXRHoverInteractable interactable)
    {
        return base.CanHover(interactable);
    }

    protected override bool CanHoverSnap(IXRInteractable interactable)
    {
        return base.CanHoverSnap(interactable);
    }

    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        return base.CanSelect(interactable);
    }

    protected override void CreateDefaultHoverMaterials()
    {
        base.CreateDefaultHoverMaterials();
    }

    protected override void DrawHoveredInteractables()
    {
        base.DrawHoveredInteractables();
    }

    public override void EndManualInteraction()
    {
        base.EndManualInteraction();
    }

    protected override bool EndSocketSnapping(XRGrabInteractable grabInteractable)
    {
        return base.EndSocketSnapping(grabInteractable);
    }

    public override void GetValidTargets(List<IXRInteractable> targets)
    {
        base.GetValidTargets(targets);
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
    }

    protected override void OnHoverEntering(HoverEnterEventArgs args)
    {
        base.OnHoverEntering(args);
    }
}
