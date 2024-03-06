using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRSocketInteractorExtension : XRSocketInteractor
{
	// OnEnable에 실행
	protected override void OnRegistered(InteractorRegisteredEventArgs args)
    {
        base.OnRegistered(args);
    }
    // OnDisable에 실행
    protected override void OnUnregistered(InteractorUnregisteredEventArgs args)
    {
        base.OnUnregistered(args);
    }

    // 여러 주기에 실행 (Before FixedUpdate / Before Update / Before LateUpdate / Before OnBeforeRender)
    public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractor(updatePhase);
    }

    #region Update 주기에 실행 (Before Update)
    public override void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.PreprocessInteractor(updatePhase);
    }
    public override void GetValidTargets(List<IXRInteractable> targets)
    {
        base.GetValidTargets(targets);
    }
    protected override void ProcessInteractionStrength(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractionStrength(updatePhase);
    }
    #endregion

    public override bool CanHover(IXRHoverInteractable interactable) // Hover entry point
    {
        // true면 HoverEnter 자동처리
        return base.CanHover(interactable);
    }
    #region Hover Enter
    protected override void OnHoverEntering(HoverEnterEventArgs args) // First Pass
    {
        base.OnHoverEntering(args);
    }

    [SerializeField] private XRUIToolObjectManager tabletUIManager;
    protected override void OnHoverEntered(HoverEnterEventArgs args) // Second Pass
    {
        base.OnHoverEntered(args);

        var grabCollider = args.interactableObject.colliders[0];
        grabCollider.transform.root.TryGetComponent(out tabletUIManager);
    }
    #endregion
    #region HoverExit
    protected override void OnHoverExiting(HoverExitEventArgs args) // First Pass
    {
        base.OnHoverExiting(args);
    }
    protected override void OnHoverExited(HoverExitEventArgs args) // Second Pass
    {
        base.OnHoverExited(args);
    }
    #endregion
    
    public override bool CanSelect(IXRSelectInteractable interactable) // Select entry point
    {
        // true면 SelectEnter 자동처리
        return base.CanSelect(interactable) && tabletUIManager != null;
    }
    #region SelectEnter
    protected override void OnSelectEntering(SelectEnterEventArgs args) // First Pass
    {
        base.OnSelectEntering(args);
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args) // Second Pass
    {
        base.OnSelectEntered(args);

        tabletUIManager.CancelPrimaryGrab();
        tabletUIManager.isInSocket = true;
        tabletUIManager.SetRigidbodyFixed();
    }
    #endregion
    #region SelectExit
    protected override void OnSelectExiting(SelectExitEventArgs args) // First Pass
    {
        base.OnSelectExiting(args);
    }
    // EndSocketSnapping
    protected override void OnSelectExited(SelectExitEventArgs args) // Second Pass
    {
		base.OnSelectExited(args);

        tabletUIManager.isInSocket = false;
        tabletUIManager.SetRigidbodyDynamic();

        tabletUIManager = null;
    }
    #endregion

    // 1. HoverEnter
    //      OnHoverEntering > CanHoverSnap
    protected override bool CanHoverSnap(IXRInteractable interactable) // Optional
    {
        return base.CanHoverSnap(interactable);
    }

    // 1. SelectEnter
    //      SelectEntering > StartSocketSnapping
    protected override bool StartSocketSnapping(XRGrabInteractable grabInteractable)
    {
        return base.StartSocketSnapping(grabInteractable);
    }

    // 1. HoverExit
    //      'EndSocketSnapping' > OnHoverExiting > OnHoverExited
    // 2. SelectEnter
    //      OnSelectEntered > 'EndSocketSnapping' > OnHoverExiting > OnHoverExited
    // 3. SelectExit
    //      OnSelectExiting > 'EndSocketSnapping' > OnSelectExited
    protected override bool EndSocketSnapping(XRGrabInteractable grabInteractable) // Pre-process
    {
        return base.EndSocketSnapping(grabInteractable);
    }

    #region Manual Interaction - Force Select Enter/Exit
    public override void StartManualInteraction(IXRSelectInteractable interactable) // Force SelectEnter
    {
        base.StartManualInteraction(interactable);
        Debug.Log("StartManualInteraction");
    }
    public override void EndManualInteraction() // Force SelectExit
    {
        base.EndManualInteraction();
        Debug.Log("EndManualInteraction");
    }
    #endregion
}
