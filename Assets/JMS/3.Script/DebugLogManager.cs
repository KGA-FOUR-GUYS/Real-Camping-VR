using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugLogManager : MonoBehaviour
{
    public TextMeshProUGUI TextBox;

    private void OnEnable()
    {
        Application.logMessageReceived += ShowLog;
    }

    private void Start()
    {
#if UNITY_ANDROID && DEVELOPMENT_BUILD
        gameObject.SetActive(true);
#else
        gameObject.SetActive(false);
#endif
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= ShowLog;
    }

    private void ShowLog(string condition, string stackTrace, LogType type)
    {
        TextBox.text += $"[{type}] {condition.Split('\n')[0]}\n";
        Debug.Log($"[{type}] {condition}\n");
    }
}
