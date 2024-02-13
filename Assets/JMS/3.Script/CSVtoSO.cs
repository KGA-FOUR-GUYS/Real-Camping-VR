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
    private static string materialPath = $"Materials";

    [MenuItem("Cook Data/Generate Recipes", true, 1)]
    public static bool ValidateGenerateRecipes()
    {
        // GenerateRecipes() execute only in edit mode
        return !EditorApplication.isPlayingOrWillChangePlaymode;
    }

    // Colume Index (0 ~ N)
    private const int RECIPE_NAME = 0;
    private const int RECIPE_DESCRIPTION = 1;
    private const int RECIPE_INGREDIENT_NAME = 2;
    private const int RECIPE_INGREDIENT_QUANTITY = 3;
    private const int RECIPE_INGREDIENT_COOK_TYPE = 4;
    private const int RECIPE_INGREDIENT_RIPE_STATE = 5;
    private const int RECIPE_INGREDIENT_TARGET_VOLUME = 6;

    [MenuItem("Cook Data/Generate Recipes", false, 1)]
    public static void GenerateRecipes()
    {
        TextAsset csvData = Resources.Load<TextAsset>(recipeCSVPath);
        string[] lines = Regex.Split(csvData.text, "(?=(?:(?:[^\"]*\"){2})*[^\"]*$)\\n");

        RecipeSO currentRecipe = null;
        RecipeIngredient currentIngredient;

        // Ignore first(header) row
        for (int i = 1; i < lines.Length; i++)
        {
            string[] datas = lines[i].Split(',');
            bool isEOF = datas[RECIPE_NAME].Equals(string.Empty);
            bool isRecipeData = !datas[RECIPE_NAME].Equals("NULL");

            // Recipe data (0 ~ 1)
            if (!isEOF && isRecipeData)
            {
                // If new recipe found, create asset with gathered data
                if (currentRecipe != null)
                    AssetDatabase.CreateAsset(currentRecipe, $"{recipeSOPath}/{currentRecipe.name}.asset");

                // Create empty recipe
                currentRecipe = ScriptableObject.CreateInstance<RecipeSO>();
                currentRecipe.name = datas[RECIPE_NAME];
                currentRecipe.description = datas[RECIPE_DESCRIPTION];
                currentRecipe.image = Resources.Load<Sprite>($"{imagePath}/{datas[RECIPE_NAME]}");
            }
            // Recipe ingredient data (2 ~ 6)
            else if (!isEOF && !isRecipeData)
            {
                // Create empty ingredient
                currentIngredient = new RecipeIngredient();
                currentIngredient.name = datas[RECIPE_INGREDIENT_NAME];
                currentIngredient.quantity = int.Parse(datas[RECIPE_INGREDIENT_QUANTITY]);
                currentIngredient.cookType = CookingEnumsExtension.ToCookType(datas[RECIPE_INGREDIENT_COOK_TYPE]);
                currentIngredient.ripeState = CookingEnumsExtension.ToRipeState(datas[RECIPE_INGREDIENT_RIPE_STATE]);
                currentIngredient.targetVolume = float.Parse(datas[RECIPE_INGREDIENT_TARGET_VOLUME]);

                // Add ingredient to list
                currentRecipe.ingredientList.Add(currentIngredient);
            }
            else if (isEOF)
            {
                // If EOF, create asset with gathered data
                if (currentRecipe != null)
                    AssetDatabase.CreateAsset(currentRecipe, $"{recipeSOPath}/{currentRecipe.name}.asset");

                // Save created assets
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"Recipe created at '{recipeSOPath}'");
            }
        }
    }

    [MenuItem("Cook Data/Generate Ingredients", true, 0)]
    public static bool ValidateGenerateIngredients()
    {
        // GenerateIngredients() execute only in edit mode
        return !EditorApplication.isPlayingOrWillChangePlaymode;
    }

    // Colume Index (0 ~ N)
    private const int INGREDIENT_NAME = 0;
    private const int INGREDIENT_DESCRIPTION = 1;
    private const int INGREDIENT_RIPE_FOR_UNDERCOOK = 2;
    private const int INGREDIENT_RIPE_FOR_WELLDONE = 3;
    private const int INGREDIENT_RIPE_FOR_OVERCOOK = 4;
    private const int INGREDIENT_RIPE_FOR_BURN = 5;
    private const int INGREDIENT_BASE_WEIGHT = 6;
    // private const int INGREDIENT_VOLUME_WEIGHT = N; // Script로 Animation Curve 그리는 방법 찾기
    private const int INGREDIENT_MIN_VOLUME = 7;

    [MenuItem("Cook Data/Generate Ingredients", false, 0)]
    public static void GenerateIngredients()
    {
        TextAsset csvData = Resources.Load<TextAsset>(ingredientCSVPath);
        string[] lines = Regex.Split(csvData.text, "(?=(?:(?:[^\"]*\"){2})*[^\"]*$)\\n");

        IngredientSO currentIngredient = null;

        // Ignore first(header) row
        for (int i = 1; i < lines.Length; i++)
        {
            string[] datas = lines[i].Split(',');
            bool isEOF = datas[INGREDIENT_NAME].Equals(string.Empty);

            // Ingredient data (0 ~ 7)
            if (!isEOF)
            {
                // If new ingredient found, create asset with gathered data
                if (currentIngredient != null)
                    AssetDatabase.CreateAsset(currentIngredient, $"{ingredientSOPath}/{currentIngredient.name}.asset");

                // Create empty recipe
                currentIngredient = ScriptableObject.CreateInstance<IngredientSO>();
                currentIngredient.name = datas[INGREDIENT_NAME];
                currentIngredient.description = datas[INGREDIENT_DESCRIPTION];
                currentIngredient.image = Resources.Load<Sprite>($"{imagePath}/{datas[INGREDIENT_NAME]}");
                currentIngredient.ripeForUndercook = float.Parse(datas[INGREDIENT_RIPE_FOR_UNDERCOOK]);
                currentIngredient.ripeForWelldone = float.Parse(datas[INGREDIENT_RIPE_FOR_WELLDONE]);
                currentIngredient.ripeForOvercook = float.Parse(datas[INGREDIENT_RIPE_FOR_OVERCOOK]);
                currentIngredient.ripeForBurn = float.Parse(datas[INGREDIENT_RIPE_FOR_BURN]);
                currentIngredient.baseWeight = float.Parse(datas[INGREDIENT_BASE_WEIGHT]);
                currentIngredient.minVolume = float.Parse(datas[INGREDIENT_MIN_VOLUME]);
                // Set Materials
                currentIngredient.rawMaterial = Resources.Load<Material>($"{materialPath}/{datas[INGREDIENT_NAME]}_Raw");
                currentIngredient.undercookMaterial = Resources.Load<Material>($"{materialPath}/{datas[INGREDIENT_NAME]}_Undercook");
                currentIngredient.welldoneMaterial = Resources.Load<Material>($"{materialPath}/{datas[INGREDIENT_NAME]}_Welldone");
                currentIngredient.overcookMaterial = Resources.Load<Material>($"{materialPath}/{datas[INGREDIENT_NAME]}_Overcook");
                currentIngredient.burnMaterial = Resources.Load<Material>($"{materialPath}/{datas[INGREDIENT_NAME]}_Burn");
            }
            else if (isEOF)
            {
                // If new ingredient found, create asset with gathered data
                if (currentIngredient != null)
                    AssetDatabase.CreateAsset(currentIngredient, $"{ingredientSOPath}/{currentIngredient.name}.asset");

                // Save created assets
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"Ingredient created at '{ingredientSOPath}'");
            }
        }
    }
}
