using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRFPSManager : MonoBehaviour
{
    public enum OVRFrequencyType
    {
        /// <summary>
        /// 72 FPS
        /// </summary>
        Default = 72,
        /// <summary>
        /// 80 FPS
        /// </summary>
        High = 80,
        /// <summary>
        /// 90 FPS
        /// </summary>
        VeryHigh = 90,
        /// <summary>
        /// 120 FPS
        /// </summary>
        Ultra = 120,
    }

    [Tooltip("Default - 72fps\nHigh - 80fps\nVeryHigh - 90fps\nUltra - 120fps")]
    public OVRFrequencyType OVRTargetFrequency;

    private void Awake()
    {
        OVRPlugin.systemDisplayFrequency = (float)OVRTargetFrequency;
    }
}
