using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using Cooking;
using UnityEngine.Assertions;

public class SliceManager : MonoBehaviour
{
    public float minVolume;
    [Header("CheckPoint")]
    [Tooltip("�ڸ��� �˻翡 ����� Point\n" +
            "������� ���� (Start : �տ��� ������� / End : �տ��� ����)")]
    public List<Transform> CheckPoints = new List<Transform>();
    [NonSerialized] public VelocityEstimator VelocityEstimator;
    //public Transform StartCheckObj;
    //public Transform EndCheckObj;

    private Material _crossMaterial;
    [Header("Parameter")]
    public float CutForce = 5f;
    public bool IsValidCut = true;
    public bool IsSliceable = true;
    public int RaycastCount;
    public float SliceCoolTime = 0;
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
            if (Physics.Linecast(CheckPoints[i].position, CheckPoints[i + 1].position , out RaycastHit hit)
                && hit.transform.root.gameObject.TryGetComponent(out _ingredientObjectManager)  // XRIngredientObjectManager - �������
                && _ingredientObjectManager.meshCalculator.Volume > minVolume                   // Volume - �ּ� ���Ǻ��� ū��
                && !_ingredientObjectManager.isPrimaryGrabbed)                                  // Grabbed - ���� ���� ��������)
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
        GameObject rendererObj = _ingredientObjectManager.virtualObjectRenderer.gameObject;

        Vector3 velocity = VelocityEstimator.GetVelocityEstimate();
        Vector3 planeNormal = Vector3.Cross(_sliceEndPos - _sliceStartPos, velocity).normalized;
        _crossMaterial = rendererObj.GetComponent<MeshRenderer>().material;

        SlicedHull hull = rendererObj.Slice(_sliceEndPos, planeNormal);
        if (hull != null)
        {
            // �ڸ��� ��Ÿ��
            StartCoroutine(SliceCoolTime_Co(SliceCoolTime));

            // EzySlice
            GameObject upperHull = hull.CreateUpperHull(rendererObj, _crossMaterial);
            GameObject lowerHull = hull.CreateLowerHull(rendererObj, _crossMaterial);

            // Copy Ingredient Model
            GameObject upperModel = CopyGameObject(modelObj);
            GameObject lowerModel = CopyGameObject(modelObj);

            SetupComponents(upperModel, upperHull);
            SetupComponents(lowerModel, lowerHull);

            // ���� ���� �ڽ� ������Ʈ
            IngredientDataManager ingredientDataManager = _ingredientObjectManager.virtualObject.GetComponent<IngredientDataManager>();
            if (ingredientDataManager.isWhole)
            {
                if (_ingredientObjectManager.virtualObject.TryGetComponent(out SpawnObject spawnObject)
                    && spawnObject.spawner.TryGetComponent(out ObjectSpawner objectSpawner))
                {
                    objectSpawner.ReturnToPool(rendererObj);
                }
            }
            // �θ� ������Ʈ
            else
            {
                Destroy(rendererObj);
            }
        }
    }
    private GameObject CopyGameObject(GameObject origin)
    {
        return Instantiate(origin);
    }

    public void SetupComponents(GameObject modelObj, GameObject meshObj)
    {
        // Set Virtual Object
        GameObject virtualObj = modelObj.GetComponentInChildren<IngredientDataManager>().gameObject;
        GameObject virtualRendererObj = meshObj.GetComponent<MeshRenderer>().gameObject;
        Destroy(virtualObj.GetComponentInChildren<MeshFilter>());
        Destroy(virtualObj.GetComponentInChildren<MeshRenderer>());
        AddCopiedComponent(meshObj.GetComponent<MeshFilter>(), virtualRendererObj);
        AddCopiedComponent(meshObj.GetComponent<MeshRenderer>(), virtualRendererObj);
        // Set Ingredient data
        virtualObj.GetComponent<MeshCalculator>().CheckVolume();
        virtualObj.GetComponent<IngredientDataManager>().isWhole = false;
        // ChangeVirtualRenderer
        modelObj.GetComponent<XRIngredientObjectManager>().ChangeVirtualRenderer(virtualRendererObj.GetComponent<MeshRenderer>());

        // Set Physical Object
        GameObject physicalObj = modelObj.GetComponentInChildren<CenterOfMass>().gameObject;
        GameObject physicalRendererObj = meshObj.GetComponent<MeshRenderer>().gameObject;
        Destroy(physicalObj.GetComponentInChildren<MeshFilter>());
        Destroy(physicalObj.GetComponentInChildren<MeshRenderer>());
        AddCopiedComponent(meshObj.GetComponent<MeshFilter>(), physicalRendererObj);
        AddCopiedComponent(meshObj.GetComponent<MeshRenderer>(), physicalRendererObj);
        // Add Convex MeshCollider
        var meshCollider = physicalRendererObj.AddComponent<MeshCollider>();
        meshCollider.convex = true;

        // Destory meshObj
        Destroy(meshObj);

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(("Sliceable")))
        {
            IsValidCut = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(("Sliceable")))
        {
            IsValidCut = true;
        }
    }

    IEnumerator SliceCoolTime_Co(float CoolTime)
    {
        IsSliceable = false;
        yield return new WaitForSeconds(CoolTime);
        IsSliceable = true;
    }

    public T AddCopiedComponent<T>(T from, GameObject to) where T : Component
    {
        System.Type type = from.GetType(); // ���� ������Ʈ�� Ÿ���� ������
        Component copy = to.AddComponent(type); // ��� GameObject�� ���� Ÿ���� ������Ʈ�� �߰��ϰ�, �� �ν��Ͻ��� ������

        // ���� ������Ʈ�� ��� �ʵ带 �����ͼ� ����
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            // �� �ʵ��� ���� �����ͼ� ����� ������Ʈ�� ����
            field.SetValue(copy, field.GetValue(from));
        }

        return copy as T; // ����� ������Ʈ�� ��ȯ
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < CheckPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(CheckPoints[i].position, CheckPoints[i + 1].position);
        }
    }
}
