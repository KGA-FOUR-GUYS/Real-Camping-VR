using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using Cooking;

public class SliceCube : MonoBehaviour
{
    public float SliceObjectVolume;
    [Header("Transform")]
    public Transform startSlicePoint;
    public Transform endSlicePoint;
    public Transform StartCheckObj;
    public Transform EndCheckObj;

    public VelocityEstimator velocityEstimator;
    public LayerMask sliceableLayer;
    private Material CrossMaterial;
    public MeshCalculator meshcal;
    [Header("Parameter")]
    public float cutForce = 5f;
    public bool isValidCut = true;
    public bool isSliceable = true;
    public int RaycastCount;
    public float SliceCoolTime = 0;
    public Vector3 EdgeSide;
    private SpawnObject spawnobject;
    private IngredientDataManager ingredient;
    private void FixedUpdate()
    {
        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
        //if (CheckAngle())
        //{
        if (hasHit && isValidCut && isSliceable)
        {
            meshcal = hit.collider.gameObject.GetComponent<MeshCalculator>();
            spawnobject = hit.collider.gameObject.GetComponent<SpawnObject>();
            if (meshcal.Volume > SliceObjectVolume)
            {
                if (!spawnobject.isselect)
                {
                    GameObject target = hit.transform.gameObject;
                    Slice(target);
                }
            }
            // }
        }
    }
    public void Slice(GameObject target)
    {
        Vector3 velocity = velocityEstimator.GetVelocityEstimate();
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
        planeNormal.Normalize();

        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);
        CrossMaterial = target.GetComponent<MeshRenderer>().material;
        ingredient = target.GetComponent<IngredientDataManager>();
        if (hull != null)
        {
            StartCoroutine(SliceCoolTime_Co(SliceCoolTime));
            GameObject upperHull = hull.CreateUpperHull(target, CrossMaterial);
            SetupSlicedComponent(upperHull, target.transform.parent, target);
            GameObject lowerHull = hull.CreateLowerHull(target, CrossMaterial);
            SetupSlicedComponent(lowerHull, target.transform.parent.transform, target);
            if (ingredient.isWhole)
            {
                spawnobject.spawner.GetComponent<ObjectSpawner>().ReturnToPool(target);
            }
            else
            {
                Destroy(target);
            }
        }
    }
    public void SetupSlicedComponent(GameObject slicedObject, Transform parent,GameObject Target)
    {
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        //MeshRenderer mesh = slicedObject.AddComponent<MeshRenderer>();
        slicedObject.layer = LayerMask.NameToLayer("Sliceable");
        slicedObject.AddComponent<MeshCalculator>();
        SpawnObject ObjectGrabInteractable = slicedObject.AddComponent<SpawnObject>();
        ObjectGrabInteractable.interactionLayers = 1 << InteractionLayerMask.NameToLayer("DirectGrab");
        IngredientDataManager originIngredient = Target.GetComponent<IngredientDataManager>();
        IngredientDataManager copiedIngredient = CopyComponent(originIngredient, slicedObject);
        copiedIngredient.isWhole = false;
        slicedObject.transform.parent = parent;
        collider.convex = true;
        rb.AddExplosionForce(cutForce, slicedObject.transform.position, 1);
    }
    public bool CheckAngle()
    {
        RaycastHit hitinfo;
        for (int i = 0; i <= RaycastCount;i++)
        {
            float ratio = i / (float)RaycastCount;
            EdgeSide = Vector3.Lerp(StartCheckObj.position, EndCheckObj.position, ratio);
            if (Physics.Raycast(EdgeSide, transform.TransformDirection(Vector3.down), out hitinfo, Mathf.Infinity) 
                && hitinfo.collider.gameObject.layer == LayerMask.NameToLayer("Sliceable"))
            {
                return true;
            }
        }
        return false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(("Sliceable")))
        {
            isValidCut = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(("Sliceable")))
        {
            isValidCut = true;
        }
    }
    IEnumerator SliceCoolTime_Co(float CoolTime)
    {
        isSliceable = false;
        yield return new WaitForSeconds(CoolTime);
        isSliceable = true;
    }
    public T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType(); // 원본 컴포넌트의 타입을 가져옴
        Component copy = destination.AddComponent(type); // 대상 GameObject에 같은 타입의 컴포넌트를 추가하고, 그 인스턴스를 가져옴

        // 원본 컴포넌트의 모든 필드를 가져와서 복사
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            // 각 필드의 값을 가져와서 복사된 컴포넌트에 설정
            field.SetValue(copy, field.GetValue(original));
        }

        return copy as T; // 복사된 컴포넌트를 반환
    }
}
