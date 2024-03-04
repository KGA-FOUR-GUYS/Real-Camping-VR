using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRSocketInteractorExtension : XRSocketInteractor
{
    protected override void Awake()
    {
        base.Awake();
    }

	#region Start 주기에 실행 (Before Start)
	protected override void OnRegistered(InteractorRegisteredEventArgs args)
    {
        base.OnRegistered(args);
    }
	#endregion
	protected override void Start()
	{
		base.Start();
    }

    // 여러 주기에 실행 (Before FixedUpdate / Before Update / Before LateUpdate / Before OnBeforeRender)
    public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractor(updatePhase);
    }

    private void FixedUpdate()
	{

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
	private void Update()
	{
		
	}

	private void LateUpdate()
	{
        
    }


    // Select Validation
    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        Debug.Log("CanSelect");
        return base.CanSelect(interactable);
    }

    // Hover Validation
    public override bool CanHover(IXRHoverInteractable interactable)
    {
        Debug.Log("CanHover");
        return base.CanHover(interactable);
    }

    protected override void OnHoverEntering(HoverEnterEventArgs args)
    {
        base.OnHoverEntering(args);
        Debug.Log("OnHoverEntering");
    }
    // HoverSnap Validation
    protected override bool CanHoverSnap(IXRInteractable interactable)
    {
        Debug.Log("CanHoverSnap");
        return base.CanHoverSnap(interactable);
    }
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        Debug.Log("OnHoverEntered");
    }

    protected override bool EndSocketSnapping(XRGrabInteractable grabInteractable)
    {
        Debug.Log("EndSocketSnapping");
        return base.EndSocketSnapping(grabInteractable);
    }

    protected override void OnHoverExiting(HoverExitEventArgs args)
    {
        base.OnHoverExiting(args);
        Debug.Log("OnHoverExiting");
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        Debug.Log("OnHoverExited");
    }

    protected override void OnUnregistered(InteractorUnregisteredEventArgs args)
    {
        base.OnUnregistered(args);
        Debug.Log("OnUnregistered");
    }

    #region Manual Interaction
    public override void StartManualInteraction(IXRSelectInteractable interactable)
	{
		base.StartManualInteraction(interactable);
        Debug.Log("StartManualInteraction");
    }
    public override void EndManualInteraction()
    {
        base.EndManualInteraction();
        Debug.Log("EndManualInteraction");
    }
	#endregion
    
    #region Socket Hover Snapping
    
    protected override bool StartSocketSnapping(XRGrabInteractable grabInteractable)
	{
        Debug.Log("StartSocketSnapping");
        return base.StartSocketSnapping(grabInteractable);
	}
    
	#endregion
	#region Registry
	
    
	#endregion
	#region Hover
	

    // Hover Enter
    
    

    // Hover Exit
    
	
    #endregion
    #region Select
    

    // Select Enter
    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        Debug.Log("OnSelectEntering");
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
	{
		base.OnSelectEntered(args);
        Debug.Log("OnSelectEntered");
    }

    // Select Exit
    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        base.OnSelectExiting(args);
        Debug.Log("OnSelectExiting");
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
	{
		base.OnSelectExited(args);
        Debug.Log("OnSelectExited");
    }
	#endregion
}
