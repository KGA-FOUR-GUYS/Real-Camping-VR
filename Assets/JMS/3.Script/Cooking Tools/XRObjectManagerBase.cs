using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;


/// <summary>
/// 자연스러운 Physical interaction을 위한 XR Object를 구성하기 위해 사용되며 <br/>
/// Virtual/Physical이 서로 position과 rotation을 추적하는 방식으로 동작합니다 <br/>
/// <br/>
/// OnSelectEntered : Physical Object -> Virtual Object <br/>
/// OnSelectExited : Virtual Object -> Physical Object 
/// </summary>
[Serializable]
public class XRObjectManagerBase : MonoBehaviour
{
	public enum XRObjectType
	{
		OneHand = 1,
		TwoHand = 2,
	}

	public bool isPrimaryGrabbed = false;
	public bool isSecondaryGrabbed = false;

	[Header("Primary Grabbed Option")]
	public XRObjectType xrObjectType = XRObjectType.OneHand;
	public LayerMask grabbedLayer;
	[Range(.1f, 3f)] public float delayToggleLayerAfterExit = 1f;
	[Tooltip("왼손으로 잡는 경우, 보정할 회전값")]
	public Vector3 leftHandRotationOffset = Vector3.zero;
	[Tooltip("오른손으로 잡는 경우, 보정할 회전값")]
	public Vector3 rightHandRotationOffset = Vector3.zero;

	[Header("Virtual Object")]
	public bool isVirtualHandVisible = false;
	public GameObject virtualObject;
	public Renderer virtualObjectRenderer;
	public Collider grabCollider;
	[Range(.1f, 10f)] public float distanceThreshold = .1f;

	[Header("Physical Object")]
	public GameObject physicalObject;
	public Renderer physicalObjectRenderer;
	public Transform primaryAttachPoint;
	public float maxSpeed = 30f;
	[Range(0f, 1f)] public float connectedBodyMassScale = 1f;

	private Transform _virtualObjectTransform;
	private Transform _physicalObjectTransform;
	private Rigidbody _physicalObjectRigidbody;
	private LayerMask _initialLayer;
	private Transform _leftHandPhysicalTransform;
	private Rigidbody _leftHandPhysicalRigidbody;
	private Transform _rightHandPhysicalTransform;
	private Rigidbody _rightHandPhysicalRigidbody;

	protected virtual void Awake()
	{
		if (virtualObject.transform.TryGetComponent(out XRGrabInteractable grabInteractable))
		{
			grabInteractable.selectEntered.AddListener(OnGrabEntered);
			grabInteractable.selectExited.AddListener(OnGrabExited);
		}

		_virtualObjectTransform = virtualObject.transform;

		_physicalObjectTransform = physicalObject.transform;
		_physicalObjectRigidbody = physicalObject.GetComponent<Rigidbody>();

		_initialLayer = _physicalObjectTransform.gameObject.layer;

		_leftHandPhysicalTransform = GameObject.FindGameObjectWithTag("LeftHandPhysical").transform;
		_leftHandPhysicalRigidbody = _leftHandPhysicalTransform.GetComponent<Rigidbody>();
		_rightHandPhysicalTransform = GameObject.FindGameObjectWithTag("RightHandPhysical").transform;
		_rightHandPhysicalRigidbody = _rightHandPhysicalTransform.GetComponent<Rigidbody>();

		Assert.IsNotNull(grabInteractable, $"[{gameObject.name}] Can't find XRGrabInteractable component in virtual object");
		Assert.IsNotNull(virtualObject, $"[{gameObject.name}] Can't find virtual object");
		Assert.IsNotNull(virtualObjectRenderer, $"[{gameObject.name}] Can't find MeshRenderer component in virtual object");
		Assert.IsNotNull(physicalObject, $"[{gameObject.name}] Can't find physical object");
		Assert.IsNotNull(physicalObjectRenderer, $"[{gameObject.name}] Can't find MeshRenderer component in physical object");
		Assert.IsNotNull(_physicalObjectRigidbody, $"[{gameObject.name}] Can't find Rigidbody component in physical object");
	}

	protected virtual void Start()
	{
		virtualObjectRenderer.enabled = isVirtualHandVisible;
	}

	protected virtual void FixedUpdate()
	{
		if (!isPrimaryGrabbed)
		{
			MatchVirtualToolToPhysicalTool();
		}
		else if (isSecondaryGrabbed)
		{
			MatchPhysicalToolToVirtualTool();
		}
		// If tool is grabbed
		// Check distance from interacting physical hand
		// 손과 거리가 너무 멀어지면 놓치기
	}

	protected virtual void Update()
	{
		ToggleVirtualToolRenderer();
	}

	/// <summary>
	/// Show virtual object if it's too far from physical object
	/// </summary>
	private void ToggleVirtualToolRenderer()
	{
		float distance = Vector3.Distance(_virtualObjectTransform.position, _physicalObjectTransform.position);
		bool isFar = distance >= distanceThreshold;

		virtualObjectRenderer.enabled = isVirtualHandVisible && isFar;
	}

	/// <summary>
	/// Match rotation and position (Virtual Object -> Physical Object)
	/// </summary>
	private void MatchVirtualToolToPhysicalTool()
	{
		_virtualObjectTransform.rotation = _physicalObjectTransform.rotation;
		_virtualObjectTransform.position = _physicalObjectTransform.position;
	}

	/// <summary>
	/// Match rotation and position (Physical Object -> Virtual Object)
	/// </summary>
	private void MatchPhysicalToolToVirtualTool()
	{
		UpdatePhysicalToolRotation();
		UpdatePhysicalToolPosition();
	}

	/// <summary>
	/// Try to match rotation with rigidbody angularVelocity
	/// </summary>
	private void UpdatePhysicalToolRotation()
	{
		Quaternion rotationDiff = _virtualObjectTransform.rotation * Quaternion.Inverse(_physicalObjectTransform.rotation);
		rotationDiff.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

		Vector3 rotationDiffInDegree = angleInDegree * rotationAxis;

		_physicalObjectRigidbody.angularVelocity = (rotationDiffInDegree * Mathf.Deg2Rad) / Time.fixedDeltaTime;
	}

	/// <summary>
	/// Try to match position with rigidbody velocity
	/// </summary>
	private void UpdatePhysicalToolPosition()
	{
		var desiredVelocity = (_virtualObjectTransform.position - _physicalObjectTransform.position) / Time.fixedDeltaTime;
		if (desiredVelocity.magnitude > maxSpeed)
		{
			var ratio = maxSpeed / desiredVelocity.magnitude;
			desiredVelocity = new Vector3(desiredVelocity.x * ratio, desiredVelocity.y * ratio, desiredVelocity.z * ratio);
		}

		_physicalObjectRigidbody.velocity = desiredVelocity;
	}

	private IXRSelectInteractor _primaryInteractor = null;
	private IXRSelectInteractor _secondaryInteractor = null;
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
			TogglePhysicalObjectLayer();
			AttachPrimaryPointToHand(e);
		}
		// Secondary Grab Entered
		else if (!isSecondaryGrabbed)
		{
			_secondaryInteractor = e.interactorObject;
			isSecondaryGrabbed = true;
			AttachSecondaryPointToHand(e);
		}
	}

	// XR Grab Interactable Events
	public virtual void OnGrabExited(SelectExitEventArgs e)
	{
		// Primary Grab Exited
		if (isPrimaryGrabbed && !isSecondaryGrabbed)
		{
			_primaryInteractor = null;
			isPrimaryGrabbed = false;
			Invoke(nameof(TogglePhysicalObjectLayer), delayToggleLayerAfterExit);
			DetachPrimaryPointFromHand(e);
		}
		// Secondary Grab Exited
		else if (isPrimaryGrabbed && isSecondaryGrabbed)
		{
			_secondaryInteractor = null;
			isSecondaryGrabbed = false;
			DetachSecondaryPointFromHand(e);
		}
	}

	/// <summary>
	/// Change collision layer of physical object <br/>
	/// <br/>
	/// grabbed layer : OnSelectEntered with primaryHand <br/>
	/// initial layer : OnSelectExited with primaryHand
	/// </summary>
	private void TogglePhysicalObjectLayer()
	{
		int currentLayerValue = _physicalObjectTransform.gameObject.layer;

		LayerMask targetLayerMask = isPrimaryGrabbed ? grabbedLayer : _initialLayer;
		int targetLayerValue = targetLayerMask == 0
								? 0
								: targetLayerMask.value < 0
									? (int)Mathf.Log(targetLayerMask.value - 1, 2f)
									: (int)Mathf.Log(targetLayerMask.value, 2f);

		if (currentLayerValue == targetLayerValue) return;

		_physicalObjectTransform.gameObject.layer = targetLayerValue;

		// Change layer of children
		var colliders = _physicalObjectTransform.GetComponentsInChildren<Collider>();
		foreach (var collider in colliders)
		{
			// 미리 지정해준 Layer가 있다면 유지 (i.e - Grab Guidance)
			if (collider.gameObject.layer != LayerMask.NameToLayer("Default")) continue;
			collider.gameObject.layer = targetLayerValue;
		}
	}

	private Vector3 _primaryColliderLocalPosition;
	private Quaternion _primaryColliderLocalRotation;
	private FixedJoint _primaryJointToHand;
	/// <summary>
	/// Match rotation, position and create fixed joint to attach physical object to physical hand
	/// </summary>
	/// <param name="e"></param>
	private void AttachPrimaryPointToHand(SelectEnterEventArgs e)
	{
		bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");

		Transform hand = isLeftHand ? _leftHandPhysicalTransform : _rightHandPhysicalTransform;
		Transform handAttachPoint = hand.GetChild(0);

		// Physical Tool 보정
		Quaternion rotationOffset = Quaternion.Inverse(_physicalObjectTransform.rotation) * primaryAttachPoint.rotation;
		_physicalObjectTransform.rotation = handAttachPoint.rotation * Quaternion.Inverse(rotationOffset);
		_physicalObjectTransform.localRotation *= isLeftHand ? Quaternion.Euler(leftHandRotationOffset) : Quaternion.Euler(rightHandRotationOffset);
		Vector3 positionOffset = _physicalObjectTransform.position - primaryAttachPoint.position;
		_physicalObjectTransform.position = handAttachPoint.position + positionOffset;

		// Priamry Grab collider 보정
		_primaryColliderLocalPosition = grabCollider.transform.localPosition;
		_primaryColliderLocalRotation = grabCollider.transform.localRotation;
		grabCollider.transform.SetParent(primaryAttachPoint);
		grabCollider.transform.rotation = primaryAttachPoint.rotation;
		grabCollider.transform.localPosition = ConvertLocalPosition(_primaryColliderLocalRotation);

		// Hand를 기준으로 Physical Tool을 FixedJoint로 연결
		Rigidbody bodyToConnect = isLeftHand ? _leftHandPhysicalRigidbody : _rightHandPhysicalRigidbody;
		_primaryJointToHand = _physicalObjectRigidbody.gameObject.AddComponent<FixedJoint>();
		_primaryJointToHand.connectedBody = bodyToConnect;
		_primaryJointToHand.connectedMassScale = connectedBodyMassScale;

		string context = isLeftHand ? "Left Hand" : "Right Hand";
		Debug.Log($"[{transform.gameObject.name}] Secondary hand grabbed {context}");
	}

	private Vector3 ConvertLocalPosition(Quaternion rotation)
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

	/// <summary>
	/// Remove fixed joint that attach physical object to physical hand
	/// </summary>
	/// <param name="e"></param>
	private void DetachPrimaryPointFromHand(SelectExitEventArgs e)
	{
		Destroy(_primaryJointToHand);

		// Primary Grab collider 원복
		grabCollider.transform.SetParent(_virtualObjectTransform);
		grabCollider.transform.localPosition = _primaryColliderLocalPosition;
		grabCollider.transform.localRotation = _primaryColliderLocalRotation;

		bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");
		string context = isLeftHand ? "Left Hand" : "Right Hand";
		Debug.Log($"[{transform.gameObject.name}] Primary hand released {context}");
	}

	private void AttachSecondaryPointToHand(SelectEnterEventArgs e)
	{
		bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");
		string context = isLeftHand ? "Left Hand" : "Right Hand";
		Debug.Log($"[{transform.gameObject.name}] Secondary hand grabbed {context}");
	}

	private void DetachSecondaryPointFromHand(SelectExitEventArgs e)
	{
		bool isLeftHand = e.interactorObject.transform.gameObject.CompareTag("LeftHandInteractor");
		string context = isLeftHand ? "Left Hand" : "Right Hand";
		Debug.Log($"[{transform.gameObject.name}] Secondary hand released {context}");
	}
}
