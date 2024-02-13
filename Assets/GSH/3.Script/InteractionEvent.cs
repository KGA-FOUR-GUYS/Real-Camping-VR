using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionEvent : MonoBehaviour
{
    [SerializeField] private RecipeEvent recipe;

    public Recipe[] GetRecipe()
    {
        recipe.progress = DatabaseManager.instance.GetRecipes((int)recipe.line.x, (int)recipe.line.y);
        return recipe.progress;
    }
}
