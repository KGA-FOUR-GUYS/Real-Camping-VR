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
		if (_setRigidbodyFixedCo != null)
        {
			StopCoroutine(_setRigidbodyFixedCo);
			_setRigidbodyFixedCo = null;
		}

		_setRigidbodyFixedCo = SetRigidbodyFixedCo();
		StartCoroutine(_setRigidbodyFixedCo);
    }
	private IEnumerator _setRigidbodyFixedCo = null;
	private IEnumerator SetRigidbodyFixedCo()
    {
        yield return null;
        isMatchToolEnabled = false;
        physicalToolRigidbody.useGravity = false;

        yield return null;
        physicalTool.transform.rotation = virtualTool.transform.rotation;
        physicalTool.transform.position = virtualTool.transform.position;
		var positionOffset = primaryAttachPoint.position - physicalTool.transform.position;
		physicalTool.transform.position += positionOffset;
		physicalToolRigidbody.constraints = RigidbodyConstraints.FreezeAll;
		
		ToggleFullScreenUI(true);
    }

    private void ToggleFullScreenUI(bool isOn)
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

    public void SetRigidbodyDynamic()
    {
		if (_setRigidbodyDynamicCo != null)
        {
			StopCoroutine(_setRigidbodyDynamicCo);
			_setRigidbodyDynamicCo = null;
		}

		_setRigidbodyDynamicCo = SetRigidbodyDynamicCo();
		StartCoroutine(_setRigidbodyDynamicCo);
    }
	private IEnumerator _setRigidbodyDynamicCo = null;
	private IEnumerator SetRigidbodyDynamicCo()
    {
		yield return null;

		physicalToolRigidbody.useGravity = true;
        physicalToolRigidbody.constraints = RigidbodyConstraints.None;

		isMatchToolEnabled = true;

		ToggleFullScreenUI(false);
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
