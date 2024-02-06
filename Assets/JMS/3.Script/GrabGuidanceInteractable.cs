using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(LineRenderer))]
public class GrabGuidanceInteractable : XRSimpleInteractable
{
    [Header("Render Trajectory")]
    public float maxOffsetY = 1f;
    [Range(3, 200)] public int vertexCount = 100; // BezierCurve¿« Point Count

    [Header("Pull Action")]
    [Tooltip("Maximum angle to regard as pull action")]
    [Range(5f, 90f)] public float angleThreshold = 30f;
    [Tooltip("Minimum speed to regard as pull action")]
    [Range(0f, 20f)] public float speedThreshold = 5f;
    [Tooltip("Total time to finish pull action")]
    [Range(1f, 10f)] public float pullDuration = 2f;
    [Tooltip("Pull speed over startPosition(left) to endPosition(right)")]
    public AnimationCurve speedOverTrajectory;
    [Range(0, 20)] public int maxDeviation = 10;

    private LineRenderer m_lineRenderer;
    private Transform m_leftHandPhysical;
    private Transform m_rightHandPhysical;

    protected override void Awake()
    {
        base.Awake();

        TryGetComponent(out m_lineRenderer);
        m_leftHandPhysical = GameObject.FindGameObjectWithTag("LeftHandPhysical").transform;
        m_rightHandPhysical = GameObject.FindGameObjectWithTag("RightHandPhysical").transform;
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);

        var currentInteractor = args.interactorObject;
        if (currentInteractor is not XRRayInteractor) return;

        // Start Render Contour
        HighlightContour(true);
    }
    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);

        var currentInteractor = args.interactorObject;
        if (currentInteractor is not XRRayInteractor) return;

        // Stop Render Contour
        HighlightContour(false);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        var currentInteractor = args.interactorObject;
        if (currentInteractor is not XRRayInteractor) return;

        // Start Render Trajectory
        var handTransform = currentInteractor.transform;
        HighlightTrajectory(true, handTransform);

        // Start Check Pull Action
        bool isLeftHand = currentInteractor.transform.gameObject.CompareTag("LeftHandInteractor");
        ToggleCheckPullAction(true, isLeftHand);
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        var currentInteractor = args.interactorObject;
        if (currentInteractor is not XRRayInteractor) return;

        // Stop Render Trajectory
        HighlightTrajectory(false);

        // Stop Check Pull Action
        bool isLeftHand = currentInteractor.transform.gameObject.CompareTag("LeftHandInteractor");
        ToggleCheckPullAction(false);
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
    public void HighlightContour(bool isHovering)
    {
        if (!isHovering)
        {
            // Hide Contour
            return;
        }

        // Show Contour
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
    private void StopCheckPullAction()
    {
        if (_currentCheckPullAction != null)
        {
            StopCoroutine(_currentCheckPullAction);
            _currentCheckPullAction = null;
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
            var criterionDirection = -handTransform.forward;

            var isValidAngle = Vector3.Dot(moveDirection, criterionDirection) >= Mathf.Cos(angleThreshold);
            var isValidSpeed = handRigidbody.velocity.magnitude >= speedThreshold;

            if (isValidAngle && isValidSpeed)
            {
                PullObjectAlongTrajectory();
                break;
            }
        }
    }

    private void PullObjectAlongTrajectory()
    {
        // Cache last trajectory
        List<Vector3> trajectory = new List<Vector3>();
        for (int i = 0; i < m_lineRenderer.positionCount; i++)
        {
            trajectory.Add(m_lineRenderer.GetPosition(i));
        }
        
        StopDrawTrajectory();

        // Prevent redundant execution
        if (_currentPullObject != null)
        {
            StopCoroutine(_currentPullObject);
            _currentPullObject = null;
        }

        _currentPullObject = PullObject(trajectory);
        StartCoroutine(_currentPullObject);
    }
    private IEnumerator _currentPullObject = null;
    private IEnumerator PullObject(List<Vector3> trajectory)
    {
        float elapsedTime = 0f;
        while (elapsedTime < pullDuration)
        {
            yield return null;
            elapsedTime += Time.deltaTime;

            float progress = elapsedTime / pullDuration;
            int originalIndex = (int)(progress * vertexCount);
            int deviation = (int)(speedOverTrajectory.Evaluate(progress)) * maxDeviation;
            int lerpedIndex = originalIndex + deviation;

            // Prevent invalid index
            lerpedIndex = Mathf.Min(vertexCount - 1, lerpedIndex);
            transform.position = trajectory[lerpedIndex];
        }
    }
    #endregion
}
