using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cooking
{
    public abstract class CookerManager : MonoBehaviour
    {
        [field: SerializeField]
        [field: Range(0.1f, 10f)] public virtual float RipePerSecond { get; set; } = 1f;

        // Common functions for cooker (boiler / broiler / griller)
    }
}

