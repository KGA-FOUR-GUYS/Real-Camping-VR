using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Range(72, 120)] public int targetFPS = 72;

    private void Awake()
    {
        Application.targetFrameRate = targetFPS;
    }
}
