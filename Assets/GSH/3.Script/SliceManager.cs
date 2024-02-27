using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using Cooking;
using UnityEngine.Assertions;
using VHACD.Unity;

public class SliceManager : MonoBehaviour
{
    public float minVolume;
    [Header("CheckPoint")]
    [Tooltip("자르기 검사에 사용할 Point\n" +
            "순서대로 지정 (Start : 손에서 가까운쪽 / End : 손에서 먼쪽)")]
    public List<Transform> CheckPoints = new List<Transform>();
    [NonSerialized] public VelocityEstimator VelocityEstimator;
    //public Transform StartCheckObj;
    //public Transform EndCheckObj;

    private Material _crossMaterial;
    [Header("Parameter")]
    public bool IsSliceable = true;
    public float SliceCoolTime = 0;
    public float CutForce = 5f;
    public int RaycastCount;
    public Vector3 EdgeSide;

    private XRIngredientObjectManager _ingredientObjectManager;
    private Vector3 _sliceStartPos;
    private Vector3 _sliceEndPos;

    private void Awake()
    {
        // Get VelocityEstimator from endPoint
        CheckPoints[CheckPoints.Count - 1].TryGetComponent(out VelocityEstimator);

        Assert.IsFalse(CheckPoints.Count <= 1, $"[{gameObject.name}] You have to assign 2 or more CheckPoints to slice");
        Assert.IsNotNull(VelocityEstimator, $"[{gameObject.name}] Can not find VelocityEstimator component in endPoint");
    }

    private void FixedUpdate()
    {
        CheckToSlice();
    }

    private void CheckToSlice()
    {
        if (IsValidCollision(out RaycastHit hit))
        {
            Slice();
        }
    }

    private bool IsValidCollision(out RaycastHit raycastHit)
    {
        for (int i = 0; i < CheckPoints.Count - 1; i++)
        {
            if (IsSliceable
                && Physics.Linecast(CheckPoints[i].position, CheckPoints[i + 1].position , out RaycastHit hit)
                && hit.transform.parent.gameObject.TryGetComponent(out _ingredientObjectManager)    // XRIngredientObjectManager - 재료인지
                && _ingredientObjectManager.meshCalculator.Volume > minVolume                       // Volume - 최소 부피보다 큰지
				&& !_ingredientObjectManager.isPrimaryGrabbed)                                      // Grabbed - 잡지 않은 상태인지)
            {
                raycastHit = hit;
                _sliceStartPos = CheckPoints[i].position;
                _sliceEndPos = CheckPoints[i + 1].position;
                return true;
            }
        }

        raycastHit = new RaycastHit();
        return false;
    }

    public void Slice()
    {
        GameObject modelObj = _ingredientObjectManager.gameObject;

        Vector3 velocity = VelocityEstimator.GetVelocityEstimate();
        Vector3 planeNormal = Vector3.Cross(_sliceEndPos - _sliceStartPos, velocity).normalized;
        _crossMaterial = _ingredientObjectManager.virtualObjectRenderer.gameObject.GetComponent<MeshRenderer>().material;

        SlicedHull hull = _ingredientObjectManager.virtualObjectRenderer.gameObject.Slice(_sliceEndPos, planeNormal);
        if (hull != null)
        {
            // 자르기 쿨타임
            StartCoroutine(SliceCoolTime_Co(SliceCoolTime));

            // EzySlice
            GameObject upperHull = hull.CreateUpperHull(_ingredientObjectManager.virtualObjectRenderer.gameObject, _crossMaterial);
            GameObject lowerHull = hull.CreateLowerHull(_ingredientObjectManager.virtualObjectRenderer.gameObject, _crossMaterial);

			// Copy Ingredient Model
			GameObject upperModel = CopyGameObject(modelObj);
			GameObject lowerModel = CopyGameObject(modelObj);
			SetupComponents(upperModel, upperHull);
			SetupComponents(lowerModel, lowerHull);

            // Set position and rotation
            var position = modelObj.transform.position;
            var rotation = modelObj.transform.rotation;
            upperModel.transform.SetPositionAndRotation(position, rotation);
            lowerModel.transform.SetPositionAndRotation(position, rotation);

            // Put in object pool
            var objectPool = _ingredientObjectManager.GetComponent<XRIngredientObjectManager>().objectPool;
            upperModel.transform.SetParent(objectPool);
            lowerModel.transform.SetParent(objectPool);

            IngredientDataManager ingredientDataManager = _ingredientObjectManager.virtualObject.GetComponent<IngredientDataManager>();
			// New object?
			if (ingredientDataManager.isWhole)
			{
				if (_ingredientObjectManager.virtualObject.TryGetComponent(out SpawnObject spawnObject)
					&& spawnObject.spawner.TryGetComponent(out ObjectSpawner objectSpawner))
				{
					objectSpawner.ReturnToPool(modelObj);
				}
			}
			// Old object?
			else
			{
				Destroy(modelObj);
			}
		}
    }
    private GameObject CopyGameObject(GameObject origin)
    {
        return Instantiate(origin);
    }

    public void SetupComponents(GameObject modelObj, GameObject slicedHull)
    {
        var ingredientManager = modelObj.GetComponent<XRIngredientObjectManager>();

        // Set Virtual Object
        GameObject virtualObj = ingredientManager.virtualObject.gameObject;
        GameObject virtualRendererObj = ingredientManager.virtualObjectRenderer.gameObject;
        CopyMeshFilter(slicedHull.GetComponent<MeshFilter>(), virtualRendererObj.GetComponent<MeshFilter>());
        CopyMeshRenderer(slicedHull.GetComponent<MeshRenderer>(), virtualRendererObj.GetComponent<MeshRenderer>());
        // Set Ingredient data
        virtualObj.GetComponent<MeshCalculator>().CheckVolume();
        virtualObj.GetComponent<IngredientDataManager>().isWhole = false;
        // Remove Trigger Collider
        Destroy(ingredientManager.grabCollider);

        // Set Physical Object
        GameObject physicalObj = ingredientManager.physicalObject.gameObject;
        GameObject physicalRendererObj = ingredientManager.physicalObjectRenderer.gameObject;
        CopyMeshFilter(slicedHull.GetComponent<MeshFilter>(), physicalRendererObj.GetComponent<MeshFilter>());
        CopyMeshRenderer(slicedHull.GetComponent<MeshRenderer>(), physicalRendererObj.GetComponent<MeshRenderer>());
        // Remove centor of mass
        Destroy(physicalObj.GetComponent<CenterOfMass>());
        physicalObj.GetComponent<Rigidbody>().automaticCenterOfMass = true;

        // Remove Convex MeshCollider
        var colliders = physicalRendererObj.GetComponentsInChildren<MeshCollider>();
        if (colliders != null)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                Destroy(colliders[i]);
            }
        }
        if (physicalRendererObj.TryGetComponent(out ComplexCollider complexCollider))
        {
            Destroy(complexCollider);
        }

        // Add new convex MeshCollider
        var meshCollider = physicalRendererObj.AddComponent<MeshCollider>();
        meshCollider.convex = true;

        // Destory meshObj
        Destroy(slicedHull);
        physicalObj.GetComponent<Rigidbody>().AddExplosionForce(CutForce, physicalObj.transform.position, 1);
    }

    //public bool CheckAngle()
    //{
    //    RaycastHit hitinfo;
    //    for (int i = 0; i <= RaycastCount;i++)
    //    {
    //        float ratio = i / (float)RaycastCount;
    //        EdgeSide = Vector3.Lerp(StartCheckObj.position, EndCheckObj.position, ratio);
    //        if (Physics.Raycast(EdgeSide, transform.TransformDirection(Vector3.down), out hitinfo, Mathf.Infinity) 
    //            && hitinfo.collider.gameObject.layer == LayerMask.NameToLayer("Sliceable"))
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    IEnumerator SliceCoolTime_Co(float CoolTime)
    {
        IsSliceable = false;
        yield return new WaitForSeconds(CoolTime);
        IsSliceable = true;
    }

    public void CopyMeshFilter(MeshFilter from, MeshFilter to)
	{
        to.mesh = from.mesh;
        to.sharedMesh = from.mesh;
	}

    public void CopyMeshRenderer(MeshRenderer from, MeshRenderer to)
	{
        to.material = from.material;
        to.materials = from.materials;
	}

    //public void CopyComponent<T>(T from, T to) where T : Component
    //{
    //    Type type = from.GetType();
    //    Component copy = to.GetComponent(type);

    //    // 원본 컴포넌트의 모든 필드를 가져와서 복사
    //    System.Reflection.FieldInfo[] fields = type.GetFields();
    //    foreach (System.Reflection.FieldInfo field in fields)
    //    {
    //        // 각 필드의 값을 가져와서 복사된 컴포넌트에 설정
    //        field.SetValue(copy, field.GetValue(from));
    //    }
    //}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < CheckPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(CheckPoints[i].position, CheckPoints[i + 1].position);
        }
    }
}
