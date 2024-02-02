using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.InputSystem;
using Cooking;

public class SliceCube : MonoBehaviour
{
    public Transform startSlicePoint;
    public Transform endSlicePoint;
    public VelocityEstimator velocityEstimator;
    public LayerMask sliceableLayer;
    private Material CrossMaterial;
    public MeshCalculator meshcal;
    public float cutForce = 5f;
    public bool CanCut = true;
    public int RaycastCount;
    public Vector3 EdgeSide;

    private void FixedUpdate()
    {

        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
        Debug.DrawRay(startSlicePoint.position, endSlicePoint.position, Color.blue);
        if (CheckAngle())
        {
            Debug.Log(hasHit);
            if (hasHit && CanCut)
            {
                Debug.Log("¿€µø");
                meshcal = hit.collider.gameObject.GetComponent<MeshCalculator>();
                if (meshcal.Volume > 0.1f)
                {
                    GameObject target = hit.transform.gameObject;
                    Slice(target);
                }
            }
        }
    }
    public void Slice(GameObject target)
    {
        Vector3 velocity = velocityEstimator.GetVelocityEstimate();
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
        planeNormal.Normalize();

        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

        if (hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(target, CrossMaterial);
            SetupSlicedComponent(upperHull, target.transform.parent);
            GameObject lowerHull = hull.CreateLowerHull(target, CrossMaterial);
            SetupSlicedComponent(lowerHull, target.transform.parent);
            Destroy(target);
        }
    }
    public void SetupSlicedComponent(GameObject slicedObject, Transform parent)
    {
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        slicedObject.layer = LayerMask.NameToLayer("Sliceable");
        slicedObject.AddComponent<MeshCalculator>();
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
            EdgeSide = Vector3.Lerp(startSlicePoint.position, endSlicePoint.position, ratio);
            if (Physics.Raycast(EdgeSide, transform.TransformDirection(Vector3.down), out hitinfo, Mathf.Infinity) 
                && hitinfo.collider.gameObject.layer == LayerMask.NameToLayer("Sliceable"))
            {
                Debug.DrawRay(EdgeSide, transform.TransformDirection(Vector3.down), Color.green);
                return true;
            }
        }
        return false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(("Sliceable")))
        {
            CanCut = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(("Sliceable")))
        {
            CanCut = true;
        }
    }
}
