using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FpsChecker : MonoBehaviour
{
    private float deltaTime = 0f;
    [SerializeField] private TextMeshProUGUI text;
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        float ms = deltaTime * 1000f;
        text.text = string.Format("{0:0.} FPS ({1:0.0} ms)", fps, ms);
    }
}
