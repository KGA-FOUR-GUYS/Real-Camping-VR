using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRLocalRigManager : MonoBehaviour
{
    public uint playerNetId;

    [Header("Local Rig")]
    public Transform localHead;
    public Transform localLeftHand;
    public Transform localRightHand;

    [Header("Local UI Pen")]
    public GameObject localLeftHandUIPen;
    public GameObject localRightHandUIPen;
}
