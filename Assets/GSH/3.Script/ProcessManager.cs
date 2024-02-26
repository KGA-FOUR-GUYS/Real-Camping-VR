using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnerData
{
    public GameObject[] spawnerPrefabs;
    public UtensilData[] utensilData;
}
[System.Serializable]
public class UtensilData
{
    public GameObject utensil;
    public Transform utensiltransform;
}

public enum CookingProcess
{
    Cut,
    Boil,
    Fry
}

public class ProcessManager : MonoBehaviour
{
    [Header("Process")]
    public CookingProcess cookingProcess;
    public SpawnerData[] spawnerData;

    [Header("Transform")]
    public Transform[] spawnerTransforms;

    private List<GameObject> instantiatedPrefabs = new List<GameObject>();

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            Process((int)cookingProcess);
        }
    }
    private void Process(int processIndex)
    {
        DestroyInstantiatedPrefabs();
        for (int j = 0; j < spawnerData[processIndex].utensilData.Length; j++)
        {
            InstantiateAndAddToQueue(spawnerData[processIndex].utensilData[j].utensil, spawnerData[processIndex].utensilData[j].utensiltransform.position, spawnerData[processIndex].utensilData[j].utensiltransform.rotation);
        }
        for (int i = 0; i < spawnerData[processIndex].spawnerPrefabs.Length && i < spawnerTransforms.Length; i++)
        {
            InstantiateAndAddToQueue(spawnerData[processIndex].spawnerPrefabs[i], spawnerTransforms[i].position, spawnerTransforms[i].rotation);
        }
    }

    private void InstantiateAndAddToQueue(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject instance = Instantiate(prefab, position, rotation);
        instantiatedPrefabs.Add(instance);
    }

    private void DestroyInstantiatedPrefabs()
    {
        foreach (var prefabInstance in instantiatedPrefabs)
        {
            Destroy(prefabInstance);
        }
        instantiatedPrefabs.Clear();
    }
}