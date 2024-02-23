using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Cooking;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] RecipeSO currentRecipe;
    [SerializeField] IngredientManager[] Ingredients;
    int Name_Num;
    [SerializeField]int Cutting_Score = 0;
    [SerializeField]int Ripe_Score = 0;

    public static ScoreManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //레시피는 나중에 수정합니다이
        currentRecipe = Resources.Load<RecipeSO>("RecipeSO/BeefStew");
        Ingredients = FindObjectsByType<IngredientManager>(FindObjectsSortMode.None);
    }

    public void Cutting()
    {
        int Correct_Cutting = 0;
        int Total_Child_Count = 0;
        for (int i = 0; i < Ingredients.Length; i++)
        {
            for (int j = 0; j < Ingredients[i].transform.childCount; j++)
            {
                var Volnume_Datas = Ingredients[i].GetComponentsInChildren<MeshCalculator>();
                if (Volnume_Datas[j].Volume <= currentRecipe.ingredientList[Matching_Name(Ingredients[i].gameObject)].sliceVolume)
                {
                    Correct_Cutting++;
                }                
            }
            Total_Child_Count += Ingredients[i].transform.childCount;
        }
        Cutting_Score = Mathf.FloorToInt(Correct_Cutting / Total_Child_Count * 100);        
    }

    private int Matching_Name(GameObject Obj)
    {
        for (int i = 0; i < currentRecipe.ingredientList.Count; i++)
        {
            if (Obj.name.Contains($"{currentRecipe.ingredientList[i].name}"))
            {
                Name_Num = i;
                break;
            }
        }
        return Name_Num;
    }



}
