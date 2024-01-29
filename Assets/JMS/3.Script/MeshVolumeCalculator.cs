using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshVolumeCalculator : MonoBehaviour
{
    [SerializeField] float _meshVolume = 0f;
    public bool isAlwaysUpdated = false;

    private Mesh _mesh;
    private Vector3 _lastLocalScale;

    void Start()
    {
        _lastLocalScale = transform.localScale;
        _mesh = GetComponent<MeshFilter>().sharedMesh;
        _meshVolume = VolumeOfMesh(_mesh, _lastLocalScale);
    }

    private void Update()
    {
        if (!isAlwaysUpdated
            || transform.localScale == _lastLocalScale) return;

        _lastLocalScale = transform.localScale;

        if (_mesh == null)
        {
            Debug.Log($"{gameObject.name} has no MeshFilter component");
            return;
        }
            
        _meshVolume = VolumeOfMesh(_mesh, _lastLocalScale);
    }

    public float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 localScale)
    {
        var xScale = localScale.x;
        var yScale = localScale.y;
        var zScale = localScale.z;

        float v321 = p3.x * xScale * p2.y * yScale * p1.z * zScale;
        float v231 = p2.x * xScale * p3.y * yScale * p1.z * zScale;
        float v312 = p3.x * xScale * p1.y * yScale * p2.z * zScale;
        float v132 = p1.x * xScale * p3.y * yScale * p2.z * zScale;
        float v213 = p2.x * xScale * p1.y * yScale * p3.z * zScale;
        float v123 = p1.x * xScale * p2.y * yScale * p3.z * zScale;

        return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
    }

    public float VolumeOfMesh(Mesh mesh, Vector3 localScale)
    {
        float volume = 0;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i + 0]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];
            volume += SignedVolumeOfTriangle(p1, p2, p3, localScale);
        }
        return Mathf.Abs(volume);
    }
}
