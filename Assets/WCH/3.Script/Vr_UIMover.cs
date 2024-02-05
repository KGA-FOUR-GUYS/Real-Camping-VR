using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class Vr_UIMover : MonoBehaviour
{
    private GameObject hoverUI;
    private bool isHover = false;
    public void UIHoverEntered(UIHoverEventArgs e)
    {
        Debug.Log($"{e.uiObject.name} Hover Entered");
        if (e.uiObject.CompareTag("HoverUI"))
        {
            hoverUI = e.uiObject;
            isHover = true;
        }
    }

    public void UIHoverExited(UIHoverEventArgs e)
    {
        Debug.Log($"{e.uiObject.name} Hover Exited");
        if (e.uiObject.CompareTag("HoverUI"))
        {
            hoverUI = null;
            isHover = false;
        }
    }

    private void Update()
    {
        if (!isHover)
            return;
        else
        {
            
        }
    }
}
