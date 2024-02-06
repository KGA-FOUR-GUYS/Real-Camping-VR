using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Create_Obj : XRSocketInteractor
{
    [SerializeField] GameObject prefab = default;
    private Vector3 attachOffset = Vector3.zero;
    public SpawnObject currentPrefab;
    protected override void Awake()
    {
        base.Awake();
        CreateAndSelectPrefab();
    }

    protected override void OnSelectExited(SelectExitEventArgs interactable)
    {
        base.OnSelectExited(interactable);
        CreateAndSelectPrefab();
    }

    void CreateAndSelectPrefab()
    {
        SpawnObject interactable = CreatePrefab();
        SelectPrefab(interactable);
    }

    SpawnObject CreatePrefab()
    {
        currentPrefab = Instantiate(prefab, transform.position - attachOffset, transform.rotation).GetComponent<SpawnObject>();
        return currentPrefab;
    }

    void SelectPrefab(SpawnObject interactable)
    {
        if (interactable != null)
        {
            var args = new SelectEnterEventArgs(); // 수정된 부분
            interactable.SelectPrefab(args); // 수정된 부분
        }
    }
}
