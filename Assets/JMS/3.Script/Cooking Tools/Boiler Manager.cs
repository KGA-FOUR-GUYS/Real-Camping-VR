using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoilerManager : MonoBehaviour
{
    [field: SerializeField]
    [field: Range(0.1f, 10f)] public float RipePerSecond { get; private set; } = 1f;
}
