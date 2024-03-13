using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;

public class XRUIToolObjectManager : XRCookingToolObjectManager
{
	private GameObject leftUIPen;
	private GameObject rightUIPen;

    protected override void Start()
	{
		base.Start();

		leftUIPen = FindObjectOfType<XRLocalRigManager>().localLeftHandUIPen;
		rightUIPen = FindObjectOfType<XRLocalRigManager>().localRightHandUIPen;
		Assert.IsNotNull(leftUIPen);
		Assert.IsNotNull(rightUIPen);

		leftUIPen.SetActive(false);
		rightUIPen.SetActive(false);
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

	public void ToggleFullScreenUI(bool isOn)
	{
		if (isOn)
		{
			// Disable poke interaction
			leftUIPen.SetActive(false);
			rightUIPen.SetActive(false);
		}

		// Disable primary collider renderer
		grabCollider.GetComponent<Renderer>().enabled = !isOn;

		// Disable GrabGuidance
		foreach (var collider in physicalTool.GetComponent<GrabGuidanceInteractable>().colliders)
		{
			collider.enabled = !isOn;
		}

		// Do Something...
	}
}
