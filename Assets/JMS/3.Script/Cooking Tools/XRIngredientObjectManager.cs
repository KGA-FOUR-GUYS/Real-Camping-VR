using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Cooking;
using UnityEngine.Assertions;

public class XRIngredientObjectManager : XRObjectManagerBase
{
	public bool isSlicable = true;

	public Transform objectPool;
    [NonSerialized] public MeshCalculator meshCalculator = null;
    [NonSerialized] public SpawnObject spawnObject = null;

	protected override void Awake()
	{
		base.Awake();

		virtualObject.TryGetComponent(out meshCalculator);
		virtualObject.TryGetComponent(out spawnObject);

		Assert.IsNotNull(meshCalculator, $"[{gameObject.name}] Can not find MeshCalculator component in virtual object");
		Assert.IsNotNull(spawnObject, $"[{gameObject.name}] Can not find SpawnObject component in virtual object");
	}

	protected override void Start()
	{
		base.Start();

		

		// 여기서 MeshCalculator로 isSlicable 여부를 확인하는 것이 바람직함
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
