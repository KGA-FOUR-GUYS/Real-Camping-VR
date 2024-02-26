using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRIngredientObjectManager : XRCookingToolObjectManager
{
	//meshcal = hit.collider.gameObject.GetComponent<MeshCalculator>();
	//spawnobject = hit.collider.gameObject.GetComponent<SpawnObject>();

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
	}

	protected override void Update()
	{
		base.Update();
	}

	// XR Grab Interactable Events
	public override void OnGrabEntered(SelectEnterEventArgs e)
	{
		base.OnGrabEntered(e);
	}

	// XR Grab Interactable Events
	public override void OnGrabExited(SelectExitEventArgs e)
	{
		base.OnGrabExited(e);
	}
}
