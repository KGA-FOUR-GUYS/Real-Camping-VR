using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Cooking;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] RecipeSO currentRecipe;
    [SerializeField] IngredientManager[] Ingredients;

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
                //이름 매칭하는 메소드 넣어줭

                var Volnume_Datas = Ingredients[i].GetComponentsInChildren<MeshCalculator>();
                if (Volnume_Datas[j].Volume <= //타겟볼륨 가져올 수 있도록)
                {
                    // currentRecipe에 있는 재료들의 이름
                    // Ingredients에 있는 재료들의 이름
                    // 비교해야할듯?
                }
                
            }
            Total_Child_Count += Ingredients[i].transform.childCount;
        }
    }

    private int Matching_Name(GameObject Obj)
    {
        for (int i = 0; i < currentRecipe.ingredientList.Count; i++)
        {
            if (Obj.name.Contains($"{currentRecipe.ingredientList[i].name}"))
            {

            }
        }
    }



}
