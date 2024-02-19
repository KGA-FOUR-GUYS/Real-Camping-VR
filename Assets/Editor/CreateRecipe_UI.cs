using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateRecipe_UI
{
    private static string recipeSOPath = $"RecipeSO";
    private static string ingredientSOPath = $"IngredientSO";
    private static string prefabPath = $"Prefabs";

    [MenuItem("Cook Data/CreateRecipe_UI", true, 2)]
    private static bool ValidateCreateRecipe()
    {
        // CreateRecipe() can execute only in edit mode
        return !EditorApplication.isPlayingOrWillChangePlaymode;
    }

    [MenuItem("Cook Data/CreateRecipe_UI", false, 2)]
    private static void CreateRecipe()
    {
        GameObject prefab = Resources.Load<GameObject>($"{prefabPath}/Recipe_UI");
        Transform parent = GameObject.Find("Recipe_Content").transform;
        int count = parent.childCount;
        if (parent.childCount > 0)
        {
            for (int i = 0; i < count; i++)
            {
                Object.DestroyImmediate(parent.GetChild(0).gameObject);
            }
        }

        RecipeSO[] recipes = Resources.LoadAll<RecipeSO>($"{recipeSOPath}");
        for (int i = 0; i < recipes.Length; i++)
        {
            GameObject recipeGO = Object.Instantiate(prefab, parent);

            Recipe_UI recipe_UI = recipeGO.GetComponent<Recipe_UI>();
            recipe_UI.currentRecipeSO = recipes[i];
            recipe_UI.cookingImg.sprite = recipe_UI.currentRecipeSO.image;
            recipe_UI.myText.text = recipe_UI.currentRecipeSO.name;

            recipeGO.name = recipe_UI.currentRecipeSO.name;
        }

    }
}
