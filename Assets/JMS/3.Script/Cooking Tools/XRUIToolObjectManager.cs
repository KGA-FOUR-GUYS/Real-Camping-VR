using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRUIToolObjectManager : XRCookingToolObjectManager
{
	public bool isInSocket = false;
	public GameObject leftUIPen;
	public GameObject rightUIPen;

	public void SetRigidbodyFixed()
    {
        StartCoroutine(SetRigidbodyFixedCo());
    }

    private IEnumerator SetRigidbodyFixedCo()
    {
		yield return new WaitForFixedUpdate();

		physicalTool.transform.forward = virtualTool.transform.forward;
		physicalTool.transform.position = virtualTool.transform.position;

		physicalToolRigidbody.useGravity = false;
		physicalToolRigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void SetRigidbodyDynamic()
    {
        StartCoroutine(SetRigidbodyDynamicCo());
    }

    private IEnumerator SetRigidbodyDynamicCo()
    {
		yield return new WaitForFixedUpdate();

		physicalToolRigidbody.useGravity = true;
        physicalToolRigidbody.constraints = RigidbodyConstraints.None;
    }

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
		if (isInSocket)
        {
			MatchPhysicalToolToVirtualTool();
			return;
		}

		base.FixedUpdate();
	}

	protected override void Update()
    {
        if (isInSocket)
        {
			MatchPhysicalToolToVirtualTool();
			ToggleVirtualToolRenderer();
			return;
		}

		base.Update();
	}

    protected override void LateUpdate()
    {
		if (isInSocket)
        {
			MatchPhysicalToolToVirtualTool();
			return;
		}
		
		base.LateUpdate();
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
