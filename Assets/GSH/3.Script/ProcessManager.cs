using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpawnerData
{
    public string name;
    public GameObject Spawnerprefabs;
}

[System.Serializable]
public struct ToolsData
{
    public GameObject ToolPrefab;
    public Transform ToolTransform;
}

public enum CookingProcess
{
    Slice,
    Boil,
    Broil,
    Grill,
}

public class ProcessManager : MonoBehaviour
{
    public static ProcessManager instance = null;
    public RecipeSO currentRecipe;

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

    [Header("SpawnerPrefab")]
    [SerializeField]private SpawnerData[] spawnerData;

    [Header("Tools")]
    [SerializeField] public ToolsData Knife;
    [SerializeField] public ToolsData Pot;
    [SerializeField] public ToolsData Grill;

    [Header("SpawnerTransform")]
    [SerializeField] private Transform[] spawnerTransform;

    [SerializeField] List<Cooking.RecipeIngredient> progressIngrediant = new List<Cooking.RecipeIngredient>();
    [SerializeField] List<GameObject> SpawnerList = new List<GameObject>();
    
    private List<GameObject> instantiatedPrefabs = new List<GameObject>();
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Process(CookingProcess.Slice);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Process(CookingProcess.Boil);
        }
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    ChangeRecipe(0);
        //}
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    ChangeRecipe(1);
        //}
    }
    public void Process(CookingProcess processIndex)
    {
        switch (processIndex)
        {
            case CookingProcess.Slice:
                SliceProcess();
                return;            
            case CookingProcess.Boil:
                BoilToolSpawn();
                return;
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

    public void SelectRecipe(RecipeSO recipe)
    {
        currentRecipe = recipe;
        progressIngrediant.Clear();
        progressIngrediant = recipe.ingredientList;
    }
    public void SliceProcess()
    {
        for(int i = 0; i<progressIngrediant.Count; i++)
        {
            if(progressIngrediant[i].sliceCount > 0)
            {
                SpawnerList.Add(CompareNameCheck(progressIngrediant[i].name));
            }
        }
        for(int i = 0;i<SpawnerList.Count && i < spawnerTransform.Length; i++)
        {
            Instantiate(SpawnerList[i], spawnerTransform[i].transform.position, spawnerTransform[i].rotation);
        }
        CutToolSpawn();
    }
    public void CutToolSpawn()
    {
        Instantiate(Knife.ToolPrefab, Knife.ToolTransform.position, Knife.ToolPrefab.transform.rotation);
    }
    public void BoilToolSpawn()
    {
        Instantiate(Pot.ToolPrefab, Pot.ToolTransform.position, Pot.ToolPrefab.transform.rotation);
    }
    private GameObject CompareNameCheck(string name)
    {
        for(int i=0; i < spawnerData.Length;i++)
        {
            if(spawnerData[i].name == name)
            {
                return spawnerData[i].Spawnerprefabs;
            }
        }
        return null;
    }
}