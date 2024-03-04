using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableWhenOutOfSight : MonoBehaviour
{
    private Camera mainCam;
    private Terrain terrain;
    private TerrainCollider terrainCollider;

    private void Awake()
    {
        mainCam = Camera.main;
        TryGetComponent(out terrain);
        TryGetComponent(out terrainCollider);
    }

    private void Update()
    {
        var isEnable = IsOutOfSight();
        terrain.enabled = isEnable;
        terrainCollider.enabled = isEnable;
    }

    private bool IsOutOfSight()
    {
        var terrainCenterPos = transform.position
                                + Vector3.right * terrain.terrainData.size.x / 2
                                + Vector3.forward * terrain.terrainData.size.z / 2;
        var difference = (mainCam.transform.position - terrainCenterPos);

        // When terrain is too close
        var distanceThreshold = Mathf.Min(terrain.terrainData.size.x / 2, terrain.terrainData.size.z / 2);
        if (Vector3.SqrMagnitude(difference) <= distanceThreshold * distanceThreshold)
        {
            return true;
        }

        // When terrain far enough
        var terrainToCamDirection = (mainCam.transform.position - terrainCenterPos).normalized;
        return Vector3.Dot(mainCam.transform.forward, terrainToCamDirection) <= 0;
    }
}
