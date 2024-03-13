using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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
    Grill
}

public class ProcessManager : NetworkBehaviour
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

    public List<ToolsData> sliceTools = new List<ToolsData>();
    public List<ToolsData> broilTools = new List<ToolsData>();
    public List<ToolsData> boilTools = new List<ToolsData>();
    public List<ToolsData> grillTools = new List<ToolsData>();

    [Header("SpawnerTransform")]
    [SerializeField] private Transform[] spawnerTransform;

    [SerializeField] List<Cooking.RecipeIngredient> progressIngrediant = new List<Cooking.RecipeIngredient>();
    [SerializeField] List<GameObject> SpawnerList = new List<GameObject>();
    
    private List<GameObject> instantiatedPrefabs = new List<GameObject>();

    //private void Start()
    //{
    //    progressIngrediant = currentRecipe.ingredientList;
    //}

    //private void Update()
    //{
    //    //if (Input.GetKeyDown(KeyCode.F))
    //    //{
    //    //    Process(CookingProcess.Slice);
    //    //}
    //    //if (Input.GetKeyDown(KeyCode.D))
    //    //{
    //    //    Process(CookingProcess.Boil);
    //    //}
    //    //if(Input.GetKeyDown(KeyCode.A))
    //    //{
    //    //    DestroyInstantiatedPrefabs();
    //    //}
    //    ////if (Input.GetKeyDown(KeyCode.A))
    //    ////{
    //    ////    ChangeRecipe(0);
    //    ////}
    //    ////if (Input.GetKeyDown(KeyCode.S))
    //    ////{
    //    ////    ChangeRecipe(1);
    //    ////}
    //}

    public void Process(CookingProcess process)
    {
        SpawnerList.Clear();
        switch (process)
        {
            case CookingProcess.Slice:
                SpawnTool(sliceTools);
                return;
            case CookingProcess.Broil:
                SpawnTool(broilTools);
                return;
            case CookingProcess.Boil:
                SpawnTool(boilTools);
                return;
        }
    }

    private void InstantiateAndAddToQueue(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject gameObj = Instantiate(prefab, position, rotation);
        NetworkServer.Spawn(gameObj);
        instantiatedPrefabs.Add(gameObj);
    }

    public void DestroyInstantiatedPrefabs()
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
        progressIngrediant = recipe.ingredientList;
    }
	private void ArrangeIngredients()
	{
		for (int i = 0; i < progressIngrediant.Count; i++)
		{
			if (progressIngrediant[i].sliceCount > 0)
			{
                var ingredient = GetValidIngredient(progressIngrediant[i].name);
                if (ingredient != null)
                    SpawnerList.Add(ingredient);
			}
		}
		for (int i = 0; i < SpawnerList.Count && i < spawnerTransform.Length; i++)
		{
			InstantiateAndAddToQueue(SpawnerList[i], spawnerTransform[i].transform.position, spawnerTransform[i].rotation);
		}
	}
    private GameObject GetValidIngredient(string name)
    {
        for (int i = 0; i < spawnerData.Length; i++)
        {
            if (spawnerData[i].name == name)
            {
                return spawnerData[i].Spawnerprefabs;
            }
        }
        return null;
    }
    public void SpawnTool(List<ToolsData> toolsData)
    {
        ArrangeIngredients();
        for (int i = 0; i < toolsData.Count; i++)
        {
            InstantiateAndAddToQueue(toolsData[i].ToolPrefab, toolsData[i].ToolTransform.position, toolsData[i].ToolTransform.rotation);
        }
    }
}
