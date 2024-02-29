using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Cooking;
using UnityEngine.Assertions;

public class XRIngredientObjectManager : XRObjectManagerBase
{
	[Header("Slice")]
	public bool isSlicable = true;
	public Transform objectPool;
    [NonSerialized] public MeshCalculator meshCalculator = null;
    [NonSerialized] public SpawnObject spawnObject = null;

	protected override void Awake()
	{
		base.Awake();

		physicalObject.TryGetComponent(out meshCalculator);
		virtualObject.TryGetComponent(out spawnObject);

		Assert.IsNotNull(meshCalculator, $"[{gameObject.name}] Can not find MeshCalculator component in physical object");
		Assert.IsNotNull(spawnObject, $"[{gameObject.name}] Can not find SpawnObject component in virtual object");
	}

	protected override void Start()
	{
		base.Start();
		meshCalculator.CheckVolume();
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
		if (grabCollider == null) return;

		base.OnGrabEntered(e);
	}

	// XR Grab Interactable Events
	public override void OnGrabExited(SelectExitEventArgs e)
	{
		base.OnGrabExited(e);
	}
}
