using UnityEngine;
using UnityEditor;
using Cooking;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;

public class CSVtoSO
{
    // File Path
    private static string recipeCSVPath = $"CSVs/RecipeCSV";
    private static string recipeSOPath = $"Assets/Resources/RecipeSO";

    private static string ingredientCSVPath = $"CSVs/IngredientCSV";
    private static string ingredientSOPath = $"Assets/Resources/IngredientSO";

    private static string imagePath = $"Images";
    private static string materialPath = $"Materials";

    [MenuItem("Cook Data/Generate Recipes", true, 1)]
    public static bool ValidateGenerateRecipes()
    {
        // GenerateRecipes() can execute only in edit mode
        return !EditorApplication.isPlayingOrWillChangePlaymode;
    }

    // Colume Index (0 ~ 4) - Recipe
    private const int RECIPE_NAME = 0;
    private const int RECIPE_DESCRIPTION = 1;
    private const int RECIPE_REQUIRED_LEVEL = 2;
    private const int RECIPE_REWARD_EXP = 3;
    private const int RECIPE_REWARD_MONEY = 4;

    // Colume Index (5 ~ 12) - Ingredients in Recipe
    private const int RECIPE_INGREDIENT_NAME = 5;
    private const int RECIPE_INGREDIENT_QUANTITY = 6;
    private const int RECIPE_INGREDIENT_RIPE_BY_BROIL = 7;
    private const int RECIPE_INGREDIENT_RIPE_BY_BOIL = 8;
    private const int RECIPE_INGREDIENT_RIPE_BY_GRILL = 9;
    private const int RECIPE_INGREDIENT_RIPE_STATE = 10;
    private const int RECIPE_INGREDIENT_SLICE_VOLUME = 11;
    private const int RECIPE_INGREDIENT_SLICE_COUNT = 12;

    [MenuItem("Cook Data/Generate Recipes", false, 1)]
    public static void GenerateRecipes()
    {
        TextAsset csvData = Resources.Load<TextAsset>(recipeCSVPath);
        string[] lines = Regex.Split(csvData.text, "(?=(?:(?:[^\"]*\"){2})*[^\"]*$)\\n");

        // Validation
        if (!IsValidRecipeIngredient(lines, out List<string> invalidIngredients))
        {
            StringBuilder context = new StringBuilder();
            context.AppendLine($"Invalid ingredient found in Assets/Resources/{recipeCSVPath}");
            context.Append($"Please check -> ");
            for (int i = 0; i < invalidIngredients.Count; i++)
            {
                // Last element
                if (i == invalidIngredients.Count - 1)
                    context.Append($"{invalidIngredients[i]}");
                else
                    context.Append($"{invalidIngredients[i]}, ");
            }

            Debug.LogError(context.ToString());
            return;
        }

        RecipeSO currentRecipe = null;
        RecipeIngredient currentIngredient;

        // Ignore first(header) row
        for (int i = 1; i < lines.Length; i++)
        {
            string[] datas = lines[i].Split(',');
            bool isEOF = string.IsNullOrEmpty(datas[RECIPE_NAME]) || string.IsNullOrWhiteSpace(datas[RECIPE_NAME]);
            bool isRecipeData = !datas[RECIPE_NAME].Equals("NULL");

            // Colume Index (0 ~ 4) - Recipe
            if (isRecipeData)
            {
                TryCreateScripatbleObject(currentRecipe);

                if (isEOF)
                {
                    // Save created assets
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    Debug.Log($"Recipe created at '{recipeSOPath}'");
                    return;
                }

                // Create empty recipe
                currentRecipe = ScriptableObject.CreateInstance<RecipeSO>();
                currentRecipe.image = Resources.Load<Sprite>($"{imagePath}/{datas[RECIPE_NAME]}");
                currentRecipe.name = datas[RECIPE_NAME];
                currentRecipe.description = datas[RECIPE_DESCRIPTION].Replace("\"", string.Empty);
                currentRecipe.requiredLevel = int.Parse(datas[RECIPE_REQUIRED_LEVEL]);
                currentRecipe.rewardExp = int.Parse(datas[RECIPE_REWARD_EXP]);
                currentRecipe.rewardMoney = int.Parse(datas[RECIPE_REWARD_MONEY]);
            }
            // Colume Index (5 ~ 12) - Ingredients in Recipe
            else
            {
                // Create empty ingredient
                currentIngredient = new RecipeIngredient();
                currentIngredient.name = datas[RECIPE_INGREDIENT_NAME];
                currentIngredient.quantity = int.Parse(datas[RECIPE_INGREDIENT_QUANTITY]);
                currentIngredient.ripeByBroil = float.Parse(datas[RECIPE_INGREDIENT_RIPE_BY_BROIL]);
                currentIngredient.ripeByBoil = float.Parse(datas[RECIPE_INGREDIENT_RIPE_BY_BOIL]);
                currentIngredient.ripeByGrill = float.Parse(datas[RECIPE_INGREDIENT_RIPE_BY_GRILL]);
                currentIngredient.ripeState = CookingEnumsExtension.ToRipeState(datas[RECIPE_INGREDIENT_RIPE_STATE]);
                currentIngredient.sliceVolume = float.Parse(datas[RECIPE_INGREDIENT_SLICE_VOLUME]);
                currentIngredient.sliceCount = int.Parse(datas[RECIPE_INGREDIENT_SLICE_COUNT]);

                // Add ingredient to list
                currentRecipe.ingredientList.Add(currentIngredient);
            }
        }
    }

    private static bool IsValidRecipeIngredient(string[] csvLines, out List<string> invalidIngredients)
    {
        IngredientSO[] ingredientSOs = Resources.FindObjectsOfTypeAll<IngredientSO>();
        List<string> invalidNames = new List<string>();

        // Ignore first(header) row
        for (int i = 1; i < csvLines.Length; i++)
        {
            // Ignore empty row
            if (string.IsNullOrEmpty(csvLines[i]) || string.IsNullOrWhiteSpace(csvLines[i])) continue;
            var name = csvLines[i].Split(',')[RECIPE_INGREDIENT_NAME];

            // Ignore empty name
            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name)) continue;

            // Ignore recipe data
            if (name.Equals("NULL")) continue;

            // Validate recipe ingredient data
            bool isInvalid = true;
            foreach (var ingredientSO in ingredientSOs)
            {
                if (name.Equals(ingredientSO.name))
                    isInvalid = false;
            }

            // Add invalid ingredient name
            if (isInvalid && !invalidNames.Contains(name))
                invalidNames.Add(name);
        }

        invalidIngredients = invalidNames;

        return invalidNames.Count == 0;
    }

    [MenuItem("Cook Data/Generate Ingredients", true, 0)]
    public static bool ValidateGenerateIngredients()
    {
        // GenerateIngredients() can execute only in edit mode
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
            bool isEOF = string.IsNullOrEmpty(datas[INGREDIENT_NAME]) || string.IsNullOrWhiteSpace(datas[INGREDIENT_NAME]);

            if (isEOF)
            {
                // Save created assets
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"Ingredient created at '{ingredientSOPath}'");
                return;
            }

            // Create empty ingredient
            currentIngredient = ScriptableObject.CreateInstance<IngredientSO>();
            currentIngredient.image = Resources.Load<Sprite>($"{imagePath}/{datas[INGREDIENT_NAME]}");
            currentIngredient.name = datas[INGREDIENT_NAME];
            currentIngredient.description = datas[INGREDIENT_DESCRIPTION].Replace("\"", string.Empty);
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

            TryCreateScripatbleObject(currentIngredient);
        }
    }

    private static bool TryCreateScripatbleObject(ScriptableObject so)
    {
        if (so is IngredientSO)
            AssetDatabase.CreateAsset(so, $"{ingredientSOPath}/{so.name}.asset");
        else if (so is RecipeSO)
            AssetDatabase.CreateAsset(so, $"{recipeSOPath}/{so.name}.asset");
        else
            return false;

        return true;
    }
}
