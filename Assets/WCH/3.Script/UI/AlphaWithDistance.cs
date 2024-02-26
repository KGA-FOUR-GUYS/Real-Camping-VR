using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


[RequireComponent(typeof(CanvasGroup))]
public class AlphaWithDistance : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    [Range(1, 30)]
    [SerializeField] private int frameInterval = 3;

    private float distance;
    private float angle;

    [SerializeField] private float maxalphaDistance = 10f;
    [SerializeField] private float disappearDistance = 20f;

    [SerializeField] private float maxalphaAngle = 75f;
    [SerializeField] private float disappearAngle = 90f;

    private void Awake()
    {
        TryGetComponent(out canvasGroup);

        //Assert.IsTrue(transform.root.Equals(transform), $"{gameObject.name} is not root object");
    }

    private void Update()
    {
        if (Time.frameCount % frameInterval != 0) return;

        CheckDistanceAngle();
    }


    private void CheckDistanceAngle()
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        angle = Vector3.Angle(transform.forward, transform.position - Camera.main.transform.position);

        ChangeAlpha();

    }

    private void ChangeAlpha()
    {
        float value = 1f;
        if (distance > maxalphaDistance)
        {
            value = Mathf.Lerp(1, 0, Mathf.InverseLerp(maxalphaDistance, disappearDistance, distance));
        }
        if (angle > maxalphaAngle && value != 0f)
        {
            float weight = Mathf.InverseLerp(disappearAngle, maxalphaAngle, angle);
            value *= weight;
        }

        canvasGroup.alpha = value;
    }
}
