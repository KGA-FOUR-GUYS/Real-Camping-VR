using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRUIToolManager : XRCookingToolManager
{
	public GameObject leftUIPen;
	public GameObject rightUIPen;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();

		leftUIPen.SetActive(false);
		rightUIPen.SetActive(false);
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

		bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");
        if (isLeftHand)
        {
			rightUIPen.SetActive(true);
        }
        else
        {
			leftUIPen.SetActive(true);
		}
	}

	// XR Grab Interactable Events
	public override void OnGrabExited(SelectExitEventArgs e)
	{
		base.OnGrabExited(e);

		bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");
		if (isLeftHand)
		{
			rightUIPen.SetActive(false);
		}
		else
		{
			leftUIPen.SetActive(false);
		}
	}
}
