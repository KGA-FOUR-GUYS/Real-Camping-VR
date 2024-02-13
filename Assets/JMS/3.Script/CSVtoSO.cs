using UnityEngine;
using UnityEditor;
using Cooking;
using System.Text.RegularExpressions;

public class CSVtoSO
{
    // File Path
    private static string recipeCSVPath = $"CSVs/RecipeCSV";
    private static string recipeSOPath = $"Assets/JMS/2.Model/Prefabs/Recipe";

    private static string ingredientCSVPath = $"CSVs/IngredientCSV";
    private static string ingredientSOPath = $"Assets/JMS/2.Model/Prefabs/Ingredient";

    private static string imagePath = $"Images";

    // Colume Index (0 ~ N)
    private const int RECIPE_NAME = 0;
    private const int RECIPE_DESCRIPTION = 1;
    private const int RECIPE_IMAGE_NAME = 2;
    private const int INGREDIENT_NAME = 3;
    private const int INGREDIENT_QUANTITY = 4;
    private const int INGREDIENT_COOK_TYPE = 5;
    private const int INGREDIENT_RIPE_STATE = 6;
    private const int INGREDIENT_TARGET_VOLUME = 7;

    [MenuItem("Cook Data/Generate Recipes", true, 1)]
    // GenerateRecipes 실행 전, 유효성 검사
    public static bool ValidateGenerateRecipes()
    {
        // Execute only in edit mode
        return !EditorApplication.isPlayingOrWillChangePlaymode;
    }

    [MenuItem("Cook Data/Generate Recipes", false, 1)]
    public static void GenerateRecipes()
    {
        TextAsset csvData = Resources.Load<TextAsset>(recipeCSVPath);
        string[] lines = Regex.Split(csvData.text, "(?=(?:(?:[^\"]*\"){2})*[^\"]*$)\\n");

        RecipeSO currentRecipe = null;
        RecipeIngredient currentIngredient;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] datas = lines[i].Split(',');

            // Recipe Info (0 ~ 2)
            if (!datas[RECIPE_NAME].Equals("NULL"))
            {
                // Add to asset folder
                if (currentRecipe != null)
                {
                    AssetDatabase.CreateAsset(currentRecipe, $"{recipeSOPath}/{currentRecipe.name}.asset");
                }

                // End Of File
                if (datas[RECIPE_NAME].Equals(string.Empty))
                {
                    // Save created assets
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    Debug.Log($"Recipe created at '{recipeSOPath}'");
                    return;
                }

                // Create empty recipe
                currentRecipe = ScriptableObject.CreateInstance<RecipeSO>();
                // Set name
                currentRecipe.name = datas[RECIPE_NAME];
                // Set description
                currentRecipe.description = datas[RECIPE_DESCRIPTION];
                // Set image
                currentRecipe.image = Resources.Load<Sprite>($"{imagePath}/{datas[RECIPE_IMAGE_NAME]}");
            }
            // Ingredient Info (3 ~ 7)
            else
            {
                // Create empty ingredient
                currentIngredient = new RecipeIngredient();
                // Set name
                currentIngredient.name = datas[INGREDIENT_NAME];
                // Set quantity
                currentIngredient.quantity = int.Parse(datas[INGREDIENT_QUANTITY]);
                // Set cookType
                currentIngredient.cookType = CookingEnumsExtension.ToCookType(datas[INGREDIENT_COOK_TYPE]);
                // Set ripeState
                currentIngredient.ripeState = CookingEnumsExtension.ToRipeState(datas[INGREDIENT_RIPE_STATE]);
                // Set targetVolume
                currentIngredient.targetVolume = float.Parse(datas[INGREDIENT_TARGET_VOLUME]);

                // Add ingredient to recipe
                currentRecipe.ingredientList.Add(currentIngredient);
            }
        }
    }

    [MenuItem("Cook Data/Generate Ingredients", true, 0)]
    // GenerateIngredients 실행 전, 유효성 검사
    public static bool ValidateGenerateIngredients()
    {
        // Execute only in edit mode
        return !EditorApplication.isPlayingOrWillChangePlaymode;
    }

    [MenuItem("Cook Data/Generate Ingredients", false, 0)]
    public static void GenerateIngredients()
    {
        
    }
}
