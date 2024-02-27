using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeData
{
    public int Id;
    public string Recipename;
    public ProcessData[] Processdata;
}

[System.Serializable]
public class ProcessData
{
    public CookingProcess _process;
    public SpawnerData Spawnerdata;
}

[System.Serializable]
public class SpawnerData
{
    public GameObject[] Spawnerprefabs;
    public UtensilData[] Utensildata;
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
    public static ProcessManager instance = null;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            return;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("Process")]
    //public CookingProcess cookingProcess;
    [SerializeField]private RecipeData[] RecipeData;

    [Header("Transform")]
    public Transform[] spawnerTransforms;

    private List<GameObject> instantiatedPrefabs = new List<GameObject>();
    public int CurrentRecipeId = 0;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Process(CookingProcess.Boil);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Process(CookingProcess.Cut);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeRecipe(0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ChangeRecipe(1);
        }
    }
    private void Process(CookingProcess processIndex)
    {
        DestroyInstantiatedPrefabs();
        int recipeid = RecipeData[CurrentRecipeId].Id;
        int idx = (int)processIndex;
        for (int j = 0; j < RecipeData[recipeid].Processdata[idx].Spawnerdata.Utensildata.Length; j++)
        {
            InstantiateAndAddToQueue(RecipeData[recipeid].Processdata[idx].Spawnerdata.Utensildata[j].utensil
                                    , RecipeData[recipeid].Processdata[idx].Spawnerdata.Utensildata[j].utensiltransform.position
                                    , RecipeData[recipeid].Processdata[idx].Spawnerdata.Utensildata[j].utensiltransform.rotation);
        }
        for (int i = 0; i < RecipeData[recipeid].Processdata[idx].Spawnerdata.Spawnerprefabs.Length && i < spawnerTransforms.Length; i++)
        {
            InstantiateAndAddToQueue(RecipeData[recipeid].Processdata[idx].Spawnerdata.Spawnerprefabs[i]
                                    , spawnerTransforms[i].position
                                    , spawnerTransforms[i].rotation);
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
    public void ChangeRecipe(int recipeID)
    {
        CurrentRecipeId = recipeID;
    }
}