using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakerParticleManager : MonoBehaviour
{
    [Header("Cool Down")]
    public float coolDown = .5f;

    [Header("Downward Validation")]
    [Range(90, 180)] public float validDownAngle = 140f;

    [Header("Movement Validation")]
    [Range(1f, 20f)] public float validSpeed = 3f;
    [Range(10f, 90f)] public float validForwardAngle = 60f;

    private Rigidbody _rigidbody;
    private ParticleSystem _particleSystem;
    private void Awake()
    {
        transform.parent.TryGetComponent(out _rigidbody);
        TryGetComponent(out _particleSystem);
    }

    private void Update()
    {
        coolDown = Mathf.Max(0, coolDown - Time.deltaTime);
        if (coolDown == 0
            && IsDownward() && IsShake())
        {
            _particleSystem.Play();
        }
    }

    private bool IsDownward()
    {
        return Vector3.Dot(transform.up, Vector3.up) < -Mathf.Cos(validDownAngle/2 * Mathf.Deg2Rad);
    }

    private bool IsShake()
    {
        var moveSpeed = _rigidbody.velocity.magnitude;
        var moveDirection = _rigidbody.velocity.normalized;

        return moveSpeed >= validSpeed && Vector3.Dot(transform.up, moveDirection) > Mathf.Cos(validForwardAngle * Mathf.Deg2Rad);
    }
}
