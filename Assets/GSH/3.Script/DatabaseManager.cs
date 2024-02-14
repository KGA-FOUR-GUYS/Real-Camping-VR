using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;

    [SerializeField] string csv_FileName;

    Dictionary<int, Recipe> recipeDic = new Dictionary<int, Recipe>();

    public static bool isFinish = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            RecipeParser theParser = GetComponent<RecipeParser>();
            Recipe[] recipe = theParser.Parse(csv_FileName);
            for(int i = 0; i <recipe.Length;i++)
            {
                recipeDic.Add(i + 1, recipe[i]);
            }
            isFinish = true;
        }
    }

    public Recipe[] GetRecipes(int _StartNum, int _EndNum)
    {
        List<Recipe> recipeList = new List<Recipe>();

        for(int i = 0; i<=_EndNum-_StartNum;i++)
        {
            recipeList.Add(recipeDic[_StartNum + i]);
        }
        return recipeList.ToArray();
    }
}
