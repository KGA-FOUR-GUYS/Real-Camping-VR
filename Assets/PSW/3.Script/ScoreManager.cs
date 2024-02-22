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
        //�����Ǵ� ���߿� �����մϴ���
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
                //�̸� ��Ī�ϴ� �޼ҵ� �־�a

                var Volnume_Datas = Ingredients[i].GetComponentsInChildren<MeshCalculator>();
                if (Volnume_Datas[j].Volume <= //Ÿ�ٺ��� ������ �� �ֵ���)
                {
                    // currentRecipe�� �ִ� ������ �̸�
                    // Ingredients�� �ִ� ������ �̸�
                    // ���ؾ��ҵ�?
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
