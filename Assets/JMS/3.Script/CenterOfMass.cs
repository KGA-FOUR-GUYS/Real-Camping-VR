using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CenterOfMass : MonoBehaviour
{
    public Transform centerOfGravity;
    private Rigidbody m_rigidbody;

    private void Awake()
    {
        TryGetComponent(out m_rigidbody);
    }

    private void Start()
    {
        if (centerOfGravity)
        {
            m_rigidbody.automaticCenterOfMass = false;
            m_rigidbody.centerOfMass = centerOfGravity.localPosition;
        }
    }
}
