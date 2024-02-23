using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ResetBtn : XRBaseInteractable
{
    [SerializeField] private ObjectSpawner spawner;
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        spawner.ResetObject(); 
    }
}
