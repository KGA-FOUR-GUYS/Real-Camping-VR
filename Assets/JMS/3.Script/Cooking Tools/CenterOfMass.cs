using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CenterOfMass : MonoBehaviour
{
    public Transform centerOfGravity;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        TryGetComponent(out _rigidbody);
    }

    private void Start()
    {
        if (centerOfGravity)
        {
            _rigidbody.automaticCenterOfMass = false;
            _rigidbody.centerOfMass = centerOfGravity.localPosition;
        }
    }
}
