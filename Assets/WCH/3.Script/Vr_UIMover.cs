using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class Vr_UIMover : MonoBehaviour
{
    public ActionBasedController xrController;
    private GameObject hoverUI;
    private Vector3 lastControllerPosition;
    private bool isHover = false;
    private bool isFirstGrip = false;
    private void Awake()
    {
        xrController = GetComponentInParent<ActionBasedController>();
    }
    public void UIHoverEntered(UIHoverEventArgs e)
    {
        if (e.uiObject.CompareTag("HoverUI"))
        {
            hoverUI = e.uiObject;
            isHover = true;
        }
    }

    public void UIHoverExited(UIHoverEventArgs e)
    {
        if (e.uiObject.CompareTag("HoverUI"))
        {
            hoverUI = null;
            isHover = false;
            isFirstGrip = false;
        }
    }

    private void Update()
    {
        if (!isHover) return;

        if (!isFirstGrip)
        {
            isFirstGrip = true;
            lastControllerPosition = xrController.transform.position;
        }

        if (xrController.selectAction.action.IsPressed())
        {
            Debug.Log("is trigger btn");
            Vector3 currentControllerPosition = xrController.transform.position;
            Vector3 deltaPosition = currentControllerPosition - lastControllerPosition;

            hoverUI.transform.parent.position += deltaPosition;

            lastControllerPosition = currentControllerPosition;
        }
    }
}
