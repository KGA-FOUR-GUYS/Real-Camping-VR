using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;
using Cooking;
using Mirror;

[Serializable]
public class XRCookingToolObjectManager : NetworkBehaviour
{
	public enum GrabType
	{
		OneHand = 0,
		TwoHand = 1,
	}

	public enum UseType
    {
		None = 0,
		Multiple = 1,
		Slice = 2,
		Broil = 3,
		Boil = 4,
		Grill = 5,
		Seasoning = 6,
    }

	[SyncVar(hook = nameof(SyncIsInSocket))]
	public bool isInSocket = false;
	private void SyncIsInSocket(bool _, bool newValue)
    {
		//if (isLocalPlayer) return;

		isInSocket = newValue;
    }

	[SyncVar(hook = nameof(SyncIsMatchToolEnabled))]
	public bool isMatchToolEnabled = true;
	private void SyncIsMatchToolEnabled(bool _, bool newValue)
    {
		//if (isLocalPlayer) return;

		isMatchToolEnabled = newValue;
    }

	[SyncVar(hook = nameof(SyncIsPrimaryGrabbed))]
	public bool isPrimaryGrabbed = false;
	private void SyncIsPrimaryGrabbed(bool _, bool newValue)
    {
		//if (isLocalPlayer) return;

		isPrimaryGrabbed = newValue;
    }

	[SyncVar(hook = nameof(SyncIsSecondaryGrabbed))]
	public bool isSecondaryGrabbed = false;
	private void SyncIsSecondaryGrabbed(bool _, bool newValue)
    {
		//if (isLocalPlayer) return;

		isSecondaryGrabbed = newValue;
    }

	[Command(requiresAuthority = false)]
	private void CmdChangePhysicalToolLayer(int targetLayerValue)
    {
		_physicalToolTransform.gameObject.layer = targetLayerValue;

		RpcChangePhysicalToolLayer(targetLayerValue);
	}
	[ClientRpc]
	private void RpcChangePhysicalToolLayer(int targetLayerValue)
    {
		//if (isLocalPlayer) return;

		_physicalToolTransform.gameObject.layer = targetLayerValue;
	}

	[Command(requiresAuthority = false)]
	private void CmdChangePhysicalToolColliderLayer(int targetLayerValue)
    {
		// Change layer of children
		var colliders = _physicalToolTransform.GetComponentsInChildren<Collider>();
		foreach (var collider in colliders)
		{
			// 미리 지정해준 Layer가 있다면 유지 (i.e - Grab Guidance)
			if (collider.gameObject.layer != LayerMask.NameToLayer("Default")) continue;

			collider.gameObject.layer = targetLayerValue;
		}

		RpcChangePhysicalToolColliderLayer(targetLayerValue);
	}
	[ClientRpc]
	private void RpcChangePhysicalToolColliderLayer(int targetLayerValue)
	{
		//if (isLocalPlayer) return;

		// Change layer of children
		var colliders = _physicalToolTransform.GetComponentsInChildren<Collider>();
		foreach (var collider in colliders)
		{
			// 미리 지정해준 Layer가 있다면 유지 (i.e - Grab Guidance)
			if (collider.gameObject.layer != LayerMask.NameToLayer("Default")) continue;

			collider.gameObject.layer = targetLayerValue;
		}
	}

	[Command(requiresAuthority = false)]
	private void CmdToggleShakerManager(bool isOn)
    {
		foreach (var shakerManager in shakerManagers)
		{
			shakerManager.gameObject.SetActive(true);
		}

		RpcToggleShakerManager(isOn);
    }
	[ClientRpc]
	private void RpcToggleShakerManager(bool isOn)
    {
		//if (isLocalPlayer) return;

		foreach (var shakerManager in shakerManagers)
		{
			shakerManager.gameObject.SetActive(true);
		}
	}

	[Command(requiresAuthority = false)]
	private void CmdChangePhysicalToolTransform(Quaternion rotation, Vector3 position)
    {
		_physicalToolTransform.rotation = rotation;
		_physicalToolTransform.position = position;

		RpcChangePhysicalToolTransform(rotation, position);
	}
	[ClientRpc]
	private void RpcChangePhysicalToolTransform(Quaternion rotation, Vector3 position)
    {
		//if (isLocalPlayer) return;

		_physicalToolTransform.rotation = rotation;
		_physicalToolTransform.position = position;
	}

	[Command(requiresAuthority = false)]
	private void CmdChangeGrabColliderTransform(Quaternion rotation, Vector3 position)
    {
		grabCollider.transform.rotation = rotation;
		grabCollider.transform.position = position;

		RpcChangeGrabColliderTransform(rotation, position);
	}
	[ClientRpc]
	private void RpcChangeGrabColliderTransform(Quaternion rotation, Vector3 position)
    {
		//if (isLocalPlayer) return;

		grabCollider.transform.rotation = rotation;
		grabCollider.transform.position = position;
	}

	[Header("Tool Charateristic")]
	public GrabType grabType = GrabType.OneHand;
	public UseType useType = UseType.None;
	public List<CookerManager> cookerManagers = new List<CookerManager>();
	public List<ShakerParticleManager> shakerManagers = new List<ShakerParticleManager>();

	[Header("Primary Grabbed Option")]
	public LayerMask grabbedLayer;
	[Range(.1f, 3f)] public float delayToggleLayerAfterExit = 1f;
	[Tooltip("왼손으로 잡는 경우, 보정할 회전값")]
	public Vector3 leftHandRotationOffset = Vector3.zero;
	[Tooltip("왼손으로 잡는 경우, 보정할 위치값")]
	public Vector3 leftHandPositionOffset = Vector3.zero;
	[Tooltip("오른손으로 잡는 경우, 보정할 회전값")]
	public Vector3 rightHandRotationOffset = Vector3.zero;
	[Tooltip("오른손으로 잡는 경우, 보정할 위치값")]
	public Vector3 rightHandPositionOffset = Vector3.zero;

	[Header("Virtual Tool")]
	public bool isVirtualHandVisible = false;
	public GameObject virtualTool;
	public Collider grabCollider;
	public Renderer virtualToolRenderer;
	[Range(.1f, 10f)] public float distanceThreshold = .1f;

	[Header("Physical Tool")]
	public GameObject physicalTool;
	public Transform primaryAttachPoint;
	public float maxSpeed = 50f;
	[Range(0f, 1f)] public float connectedBodyMassScale = 1f;

	private Transform _virtualToolTransform;

	private Transform _physicalToolTransform;
	protected Rigidbody physicalToolRigidbody;

	private LayerMask _initialLayer;

	private Transform _leftHandPhysicalTransform;
	private Rigidbody _leftHandPhysicalRigidbody;
	private Transform _rightHandPhysicalTransform;
	private Rigidbody _rightHandPhysicalRigidbody;

	protected virtual void Awake()
	{
		if (virtualTool.transform.TryGetComponent(out XRGrabInteractable grabInteractable))
		{
			grabInteractable.selectEntered.AddListener(OnGrabEntered);
			grabInteractable.selectExited.AddListener(OnGrabExited);
			grabInteractable.activated.AddListener(OnActivated);
			grabInteractable.deactivated.AddListener(OnDeactivated);
		}
		else
		{
			Assert.IsNotNull(grabInteractable, $"[{gameObject.name}] Can't find XRGrabInteractable component in virtualTool");
		}

		if (useType.Equals(UseType.Multiple))
			Assert.IsFalse(cookerManagers.Count <= 1, $"[{gameObject.name}] Can't find enough CookerManager components for cook. (2 or more CookerManager needed for Multiple cook type)");
		else if (useType.Equals(UseType.Broil))
			Assert.IsFalse(cookerManagers.Count == 0, $"[{gameObject.name}] Can't find BroilManager component for broil");
		else if (useType.Equals(UseType.Boil))
			Assert.IsFalse(cookerManagers.Count == 0, $"[{gameObject.name}] Can't find BoilManager component for boil");
		else if (useType.Equals(UseType.Grill))
			Assert.IsFalse(cookerManagers.Count == 0, $"[{gameObject.name}] Can't find GrillManager component for grill");
	}

	protected virtual void Start()
	{
		_virtualToolTransform = virtualTool.transform;

		_physicalToolTransform = physicalTool.transform;
		physicalToolRigidbody = physicalTool.GetComponent<Rigidbody>();

		_initialLayer = _physicalToolTransform.gameObject.layer;

		_leftHandPhysicalTransform = GameObject.FindGameObjectWithTag("LeftHandPhysical").transform;
		_leftHandPhysicalRigidbody = _leftHandPhysicalTransform.GetComponent<Rigidbody>();
		_rightHandPhysicalTransform = GameObject.FindGameObjectWithTag("RightHandPhysical").transform;
		_rightHandPhysicalRigidbody = _rightHandPhysicalTransform.GetComponent<Rigidbody>();
	}

	protected virtual void FixedUpdate()
    {
		if (isInSocket)
		{
			MatchPhysicalToolToVirtualTool();
			return;
		}

		MatchTool();
    }

    protected virtual void Update()
	{
		ToggleVirtualToolRenderer();

		if (isInSocket)
		{
			MatchPhysicalToolToVirtualTool();
			return;
		}

		MatchTool();
	}

	protected virtual void LateUpdate()
    {
		if (isInSocket)
		{
			MatchPhysicalToolToVirtualTool();
			return;
		}

		MatchTool();
	}

    private void MatchTool()
	{
		if (!isMatchToolEnabled) return;

		if (!isPrimaryGrabbed)
		{
			MatchVirtualToolToPhysicalTool();
		}
		else if (isSecondaryGrabbed)
		{
			MatchPhysicalToolToVirtualTool();
		}
	}
	protected void ToggleVirtualToolRenderer()
	{
		float distance = Vector3.Distance(_virtualToolTransform.position, _physicalToolTransform.position);
		bool isFar = distance >= distanceThreshold;

		virtualToolRenderer.enabled = isVirtualHandVisible && isFar;
	}
	protected void MatchVirtualToolToPhysicalTool()
	{
		_virtualToolTransform.rotation = _physicalToolTransform.rotation;
		_virtualToolTransform.position = _physicalToolTransform.position;
	}
	protected void MatchPhysicalToolToVirtualTool()
	{
		UpdatePhysicalToolRotation();
		UpdatePhysicalToolPosition();
	}
	private void UpdatePhysicalToolRotation()
	{
		// Try to match rotation (physical -> virtual)
		Quaternion rotationDiff = _virtualToolTransform.rotation * Quaternion.Inverse(_physicalToolTransform.rotation);
		rotationDiff.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

		Vector3 rotationDiffInDegree = angleInDegree * rotationAxis;

		physicalToolRigidbody.angularVelocity = (rotationDiffInDegree * Mathf.Deg2Rad) / Time.fixedDeltaTime;
	}
	private void UpdatePhysicalToolPosition()
	{
		// Try to match position (physical -> virtual)
		var desiredVelocity = (_virtualToolTransform.position - _physicalToolTransform.position) / Time.fixedDeltaTime;
		if (desiredVelocity.magnitude > maxSpeed)
		{
			var ratio = maxSpeed / desiredVelocity.magnitude;
			desiredVelocity = new Vector3(desiredVelocity.x * ratio, desiredVelocity.y * ratio, desiredVelocity.z * ratio);
		}

		physicalToolRigidbody.velocity = desiredVelocity;
	}

	private IXRInteractor _primaryInteractor = null;
	private IXRInteractor _secondaryInteractor = null;
	// XR Grab Interactable Events
	public virtual void OnGrabEntered(SelectEnterEventArgs e)
	{
		bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");
		bool isRightHand = e.interactorObject.transform.gameObject.CompareTag("RightHandInteractor");
		if (!isLeftHand && !isRightHand) return;

		// Primary Grab Entered
		if (!isPrimaryGrabbed)
        {
            _primaryInteractor = e.interactorObject;
            isPrimaryGrabbed = true;
            TogglePhysicalToolLayer();
            AttachPrimaryPointToHand(e);

            if (useType.Equals(UseType.Seasoning))
            {
				CmdToggleShakerManager(true);
            }

            StartSyncPrimaryGrab();
        }
        // Secondary Grab Entered
        else if (!isSecondaryGrabbed)
        {
            if (!grabType.Equals(GrabType.TwoHand)) return;

            _secondaryInteractor = e.interactorObject;
            isSecondaryGrabbed = true;
            AttachSecondaryPointToHand(e);
        }
    }

	private IEnumerator _currentSyncPrimaryGrab = null;
	private void StartSyncPrimaryGrab()
    {
		StopSyncPrimaryGrab();

        _currentSyncPrimaryGrab = SyncPrimaryGrab();
        StartCoroutine(_currentSyncPrimaryGrab);
    }
	private void StopSyncPrimaryGrab()
    {
		if (_currentSyncPrimaryGrab != null)
		{
			StopCoroutine(_currentSyncPrimaryGrab);
			_currentSyncPrimaryGrab = null;
		}
	}
	private IEnumerator SyncPrimaryGrab()
    {
        while (true)
        {
			yield return null;

			CmdChangePhysicalToolTransform(_physicalToolTransform.rotation, _physicalToolTransform.position);
			CmdChangeGrabColliderTransform(grabCollider.transform.rotation, grabCollider.transform.position);
		}
    }

	// XR Grab Interactable Events
	public virtual void OnGrabExited(SelectExitEventArgs e)
	{
		if (e.interactableObject.Equals(_secondaryInteractor))
        {
			// Secondary Grab Exited
			CancelSecondaryGrab();
		}
		else if(e.interactableObject.Equals(_primaryInteractor))
        {
			// Primary Grab Exited
			CancelPrimaryGrab();
			if (useType.Equals(UseType.Seasoning))
			{
				foreach (var shakerManager in shakerManagers)
				{
					CmdToggleShakerManager(false);
				}
			}

			StopSyncPrimaryGrab();
		}
    }

	public void CancelPrimaryGrab()
	{
		if (!isPrimaryGrabbed || isSecondaryGrabbed) return;

		_primaryInteractor = null;
		isPrimaryGrabbed = false;
		Invoke(nameof(TogglePhysicalToolLayer), delayToggleLayerAfterExit);
		DetachPrimaryPointFromHand();
	}

	public void CancelSecondaryGrab()
    {
		if (!isPrimaryGrabbed || !isSecondaryGrabbed) return;
		if (!grabType.Equals(GrabType.TwoHand)) return;

		_secondaryInteractor = null;
        isSecondaryGrabbed = false;
        DetachSecondaryPointFromHand();
    }

    private void TogglePhysicalToolLayer()
	{
		int currentLayerValue = _physicalToolTransform.gameObject.layer;

		LayerMask targetLayerMask = isPrimaryGrabbed ? grabbedLayer : _initialLayer;
		int targetLayerValue = targetLayerMask == 0
								? 0
								: targetLayerMask.value < 0
									? (int)Mathf.Log(targetLayerMask.value - 1, 2f)
									: (int)Mathf.Log(targetLayerMask.value, 2f);

		if (currentLayerValue == targetLayerValue) return;
		CmdChangePhysicalToolLayer(targetLayerValue);
		CmdChangePhysicalToolColliderLayer(targetLayerValue);
	}

	// XR Grab Interactable Events
	public virtual void OnActivated(ActivateEventArgs e)
    {
		// Primary hand only
		if (!_primaryInteractor.Equals(e.interactorObject)) return;

        foreach (var cookerManager in cookerManagers)
        {
			cookerManager.ToggleArea(true);
		}
	}

	// XR Grab Interactable Events
	public virtual void OnDeactivated(DeactivateEventArgs e)
    {
		// Primary hand only
		if (!_primaryInteractor.Equals(e.interactorObject)) return;

		foreach (var cookerManager in cookerManagers)
		{
			cookerManager.ToggleArea(false);
		}
	}

	private Vector3 _primaryColliderLocalPosition;
	private Quaternion _primaryColliderLocalRotation;
	private FixedJoint _primaryJointToHand;
	private void AttachPrimaryPointToHand(SelectEnterEventArgs e)
	{
		bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");

		Transform hand = isLeftHand ? _leftHandPhysicalTransform : _rightHandPhysicalTransform;
		Transform handAttachPoint = hand.GetChild(0);

		// Physical Tool 보정
		Quaternion rotationOffset = Quaternion.Inverse(_physicalToolTransform.rotation) * primaryAttachPoint.rotation;
		_physicalToolTransform.rotation = handAttachPoint.rotation * Quaternion.Inverse(rotationOffset);
		_physicalToolTransform.localRotation *= isLeftHand ? Quaternion.Euler(leftHandRotationOffset) : Quaternion.Euler(rightHandRotationOffset);
		Vector3 positionOffset = _physicalToolTransform.position - primaryAttachPoint.position;
		_physicalToolTransform.position = handAttachPoint.position + positionOffset;
		_physicalToolTransform.localPosition += isLeftHand ? leftHandPositionOffset : rightHandPositionOffset;

		// Priamry Grab collider 보정
		_primaryColliderLocalPosition = grabCollider.transform.localPosition;
		_primaryColliderLocalRotation = grabCollider.transform.localRotation;
		grabCollider.transform.SetParent(primaryAttachPoint);
		grabCollider.transform.rotation = primaryAttachPoint.rotation;
		grabCollider.transform.localPosition = ConvertLocalPosition(_primaryColliderLocalRotation);

		// Hand를 기준으로 Physical Tool을 FixedJoint로 연결
		Rigidbody bodyToConnect = isLeftHand ? _leftHandPhysicalRigidbody : _rightHandPhysicalRigidbody;
		_primaryJointToHand = physicalToolRigidbody.gameObject.AddComponent<FixedJoint>();
		_primaryJointToHand.connectedBody = bodyToConnect;
		_primaryJointToHand.connectedMassScale = connectedBodyMassScale;

		//string context = isLeftHand ? "Left Hand" : "Right Hand";
		//Debug.Log($"[{transform.gameObject.name}] Primary hand grabbed {context}");
	}
	protected Vector3 ConvertLocalPosition(Quaternion rotation)
	{
		if (rotation.Equals(Quaternion.identity)) return Vector3.zero;

		var localPosition = _primaryColliderLocalPosition - primaryAttachPoint.localPosition;
		var eulerAngle = rotation.eulerAngles;

		/// x축으로 90회전하면 아래와 같이 축이 변화한다
		///     x = x
		///     y = z = Cos(90x) * x + Sin(90x) * z
		///     z = -y = Cos(90x) * x + Sin(-90x) * y
		/// 
		/// y축으로 90회전하면 아래와 같이 축이 변화한다
		///     x = z = Cos(90y) * y + Sin(90y) * z
		///     y = y
		///     z = -x = Sin(-90y) * x + Sin(90y) * y
		///
		/// z축으로 90회전하면 아래와 같이 축이 변화한다
		///     x = y = Sin(90z) * y + Cos(90z) * z
		///     y = -x = Sin(-90z) * x + Cos(90z) * z
		///     z = z
		///     
		/// 위 식을 정리하면 아래와 같다    
		///     y = Cos(90x) * x + Sin(90x) * z
		///     z = Cos(90x) * x + Sin(-90x) * y
		///     
		/// 	x = Cos(90y) * y + Sin(90y) * z
		///     z = Sin(-90y) * x + Sin(90y) * y
		/// 	
		///     x = Sin(90z) * y + Cos(90z) * z
		///     y = Sin(-90z) * x + Cos(90z) * z

		float x, y, z;
		x = y = z = 0;

		float thetaX = eulerAngle.x * Mathf.Deg2Rad;
		float thetaY = eulerAngle.y * Mathf.Deg2Rad;
		float thetaZ = eulerAngle.z * Mathf.Deg2Rad;

		if (Mathf.Abs(eulerAngle.x) > 0) // x축 회전
		{
			y += Mathf.Cos(thetaX) * localPosition.x + Mathf.Sin(thetaX) * localPosition.z;
			z += Mathf.Cos(thetaX) * localPosition.x + Mathf.Sin(-thetaX) * localPosition.y;
		}
		if (Mathf.Abs(eulerAngle.y) > 0) // y축 회전
		{
			x += Mathf.Cos(thetaY) * localPosition.y + Mathf.Sin(thetaY) * localPosition.z;
			z += Mathf.Sin(-thetaY) * localPosition.x + Mathf.Sin(thetaY) * localPosition.y;
		}
		if (Mathf.Abs(eulerAngle.z) > 0) // z축 회전
		{
			x += Mathf.Sin(thetaZ) * localPosition.y + Mathf.Cos(thetaZ) * localPosition.z;
			y += Mathf.Sin(-thetaZ) * localPosition.x + Mathf.Cos(thetaZ) * localPosition.z;
		}

		return new Vector3(x, y, z);
	}

	private void DetachPrimaryPointFromHand()
	{
		Destroy(_primaryJointToHand);

		// Primary Grab collider 원복
		grabCollider.transform.SetParent(_virtualToolTransform);
		grabCollider.transform.localPosition = _primaryColliderLocalPosition;
		grabCollider.transform.localRotation = _primaryColliderLocalRotation;

		//bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");
		//string context = isLeftHand ? "Left Hand" : "Right Hand";
		//Debug.Log($"[{transform.gameObject.name}] Primary hand released {context}");
	}

	private void AttachSecondaryPointToHand(SelectEnterEventArgs e)
	{
		//bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");
		//string context = isLeftHand ? "Left Hand" : "Right Hand";
		//Debug.Log($"[{transform.gameObject.name}] Secondary hand grabbed {context}");
	}

	private void DetachSecondaryPointFromHand()
	{
		//bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");
		//string context = isLeftHand ? "Left Hand" : "Right Hand";
		//Debug.Log($"[{transform.gameObject.name}] Secondary hand released {context}");
	}

    #region Socket Interaction
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
	}
    #endregion
}
