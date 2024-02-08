using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Outline))]
public class GrabGuidanceInteractable : XRSimpleInteractable
{
    

    [Header("Highlight Trajectory")]
    public float maxOffsetY = 1f;
    [Range(3, 200)] public int vertexCount = 20; // BezierCurve¿« Point Count

    [Header("Pull Action")]
    [Tooltip("If false, you must unselect to trigger pull action")]
    public bool isAutoPull = false;
    [Tooltip("Maximum angle to regard as pull action")]
    [Range(5f, 90f)] public float angleThreshold = 30f;
    [Tooltip("Minimum speed to regard as pull action")]
    [Range(0f, 20f)] public float speedThreshold = 5f;
    [Tooltip("Total time to finish pull action")]
    [Range(1f, 10f)] public float duration = 2f;
    [Tooltip("start time(left) to end time(right)")]
    public AnimationCurve positionOverTime;

    private LineRenderer m_lineRenderer;
    private Outline m_outline;
    private XRCookingToolManager m_cookingToolManager;

    private Transform m_leftHandPhysical;
    private Transform m_rightHandPhysical;

    protected override void Awake()
    {
        base.Awake();

        TryGetComponent(out m_lineRenderer);
        TryGetComponent(out m_outline);
        transform.parent.TryGetComponent(out m_cookingToolManager);

        m_leftHandPhysical = GameObject.FindGameObjectWithTag("LeftHandPhysical").transform;
        m_rightHandPhysical = GameObject.FindGameObjectWithTag("RightHandPhysical").transform;
    }

    private IXRInteractor _currentInteractor = null;
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (m_cookingToolManager.isGrabbed) return;

        base.OnHoverEntered(args);

        if (_currentInteractor == null)
            HighlightContour(true);
    }
    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);

        HighlightContour(false);
    }
    
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (m_cookingToolManager.isGrabbed) return;

        base.OnSelectEntered(args);

        _currentInteractor = args.interactorObject;
        if (_currentInteractor is not XRRayInteractor) return;

        // Start Check Pull Action
        bool isLeftHand = _currentInteractor.transform.gameObject.CompareTag("LeftHandInteractor");
        ToggleCheckPullAction(true, isLeftHand);

        // Start Render Trajectory
        var handTransform = _currentInteractor.transform;
        HighlightTrajectory(true, handTransform);

        // Stop Highlight Contour
        HighlightContour(false);
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (_currentInteractor is not XRRayInteractor) return;

        // Stop Check Pull Action
        bool isLeftHand = _currentInteractor.transform.gameObject.CompareTag("LeftHandInteractor");
        ToggleCheckPullAction(false);

        // Stop Render Trajectory
        HighlightTrajectory(false);

        _currentInteractor = null;
    }

    #region HighlightTrajectory
    public void HighlightTrajectory(bool isHovering, Transform handTransform = null)
    {
        if (!isHovering)
        {
            StopDrawTrajectory();
            return;
        }

        StartDrawTrajectory(handTransform);
    }
    private void StartDrawTrajectory(Transform handTransform)
    {
        StopDrawTrajectory();

        _currentTrajectory = DrawTrajectory(handTransform);
        StartCoroutine(_currentTrajectory);
    }
    private void StopDrawTrajectory()
    {
        if (_currentTrajectory != null)
        {
            StopCoroutine(_currentTrajectory);
            _currentTrajectory = null;
        }

        m_lineRenderer.positionCount = 0;
    }
    private IEnumerator _currentTrajectory = null;
    private IEnumerator DrawTrajectory(Transform handTransform)
    {
        m_lineRenderer.positionCount = vertexCount + 1;

        while (true)
        {
            yield return null;

            var startPos = transform.position;
            var endPos = handTransform.position;
            var midPos = new Vector3((startPos.x + endPos.x) / 2, endPos.y + maxOffsetY, (startPos.z + endPos.z) / 2);
            DrawBezierCurve3(startPos, midPos, endPos);
        }
    }
    private void DrawBezierCurve3(Vector3 startPos, Vector3 midPos, Vector3 endPos)
    {
        // Start
        m_lineRenderer.SetPosition(0, startPos);

        // Between
        for (int i = 1; i < vertexCount; i++)
        {
            var p1 = Vector3.Lerp(startPos, midPos, i / (float)vertexCount);
            var p2 = Vector3.Lerp(midPos, endPos, i / (float)vertexCount);

            var drawPos = Vector3.Lerp(p1, p2, i / (float)vertexCount);
            m_lineRenderer.SetPosition(i, drawPos);
        }

        // End
        m_lineRenderer.SetPosition(vertexCount, endPos);
    }
    #endregion

    #region HighlightContour
    public void HighlightContour(bool isOn)
    {
        m_outline.enabled = isOn;
    }
    #endregion

    #region ToggleCheckPullAction
    private void ToggleCheckPullAction(bool isOn, bool isLeftHand = false)
    {
        if (!isOn)
        {
            StopCheckPullAction();
            return;
        }

        StartCheckPullAction(isLeftHand);
    }
    private void StartCheckPullAction(bool isLeftHand)
    {
        StopCheckPullAction();

        var handTransform = isLeftHand ? m_leftHandPhysical : m_rightHandPhysical;
        _currentCheckPullAction = CheckPullAction(handTransform);
        StartCoroutine(_currentCheckPullAction);
    }

    private bool _isPulling = false;
    private void StopCheckPullAction()
    {
        if (_currentCheckPullAction != null)
        {
            StopCoroutine(_currentCheckPullAction);
            _currentCheckPullAction = null;
        }

        if (_isPulling)
        {
            _isPulling = false;
            PullObjectAlongTrajectory();
        }
    }
    private IEnumerator _currentCheckPullAction = null;
    private IEnumerator CheckPullAction(Transform handTransform)
    {
        var handRigidbody = handTransform.GetComponent<Rigidbody>();

        while (true)
        {
            yield return null;

            var moveDirection = handRigidbody.velocity.normalized;
            var criterionDirection = handTransform.GetChild(0).up;

            var isValidAngle = Vector3.Dot(moveDirection, criterionDirection) >= Mathf.Cos(angleThreshold);
            var isValidSpeed = handRigidbody.velocity.magnitude >= speedThreshold;

            _isPulling = isValidAngle && isValidSpeed;

            if (isAutoPull && _isPulling)
            {
                _isPulling = false;
                PullObjectAlongTrajectory();
                break;
            }
        }
    }

    private void PullObjectAlongTrajectory()
    {
        StopDrawTrajectory();

        // Prevent redundant execution
        if (_currentPullObject != null)
        {
            StopCoroutine(_currentPullObject);
            _currentPullObject = null;
        }

        _currentPullObject = PullObject();
        StartCoroutine(_currentPullObject);
    }
    private IEnumerator _currentPullObject = null;
    private IEnumerator PullObject()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = _currentInteractor.transform.position;
        Vector3 midPos = new Vector3(
            (startPos.x + endPos.x) / 2,
            endPos.y + maxOffsetY,
            (startPos.z + endPos.z) / 2);

        float elapsedTime = 0f;
        while (!m_cookingToolManager.isGrabbed
               && elapsedTime < duration)
        {
            yield return null;
            elapsedTime += Time.deltaTime;

            float progress = positionOverTime.Evaluate(elapsedTime / duration);
            transform.position = GetPointOnBezierCurve3(startPos, midPos, endPos, progress);
        }
    }
    private Vector3 GetPointOnBezierCurve3(Vector3 startPos, Vector3 midPos, Vector3 endPos, float progress)
    {
        var p1 = Vector3.Lerp(startPos, midPos, progress);
        var p2 = Vector3.Lerp(midPos, endPos, progress);

        return Vector3.Lerp(p1, p2, progress);
    }
    #endregion
}
