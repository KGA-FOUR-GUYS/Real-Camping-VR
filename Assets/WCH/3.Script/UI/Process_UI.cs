using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Process_UI : MonoBehaviour
{
    public CookingProcess currentProcess;

    public TextMeshProUGUI title_Text;
    public void SetInfo()
    {
        title_Text = GetComponentInChildren<TextMeshProUGUI>();
        title_Text.text = currentProcess.ToString();
    }
}
