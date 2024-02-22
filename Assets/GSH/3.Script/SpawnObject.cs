using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SpawnObject : XRGrabInteractable
{
    public bool isselect = false;
    protected override void Awake()
    {
        base.Awake();
    }
    private void Update()
    {
        isselect = isSelected;
    }
    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
    }
}