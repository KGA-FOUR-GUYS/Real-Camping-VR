using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class SpawnObject : XRGrabInteractable
{
    private Rigidbody rb;
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }
    public void SelectPrefab(SelectEnterEventArgs args)
    {
        OnSelectEntered(args);
    }
    //private static void SetInteractor(SelectEnterEventArgs args, XRBaseInteractor interactor)
    //{
    //    var interactorProperty = typeof(SelectEnterEventArgs).GetProperty("interactor");
    //    interactorProperty.SetValue(args, interactor);
    //}
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
    }
}
