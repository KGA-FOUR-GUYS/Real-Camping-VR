using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cooking;

[CreateAssetMenu(fileName = "RecipeName", menuName = "ScriptableObjects/RecipeSO", order = 0)]
public class RecipeSO : ScriptableObject
{
    [Header("UI")]
    [Tooltip("레시피 UI 이미지")]
    public Sprite image;
    [Tooltip("레시피 설명")]
    [TextArea(minLines: 3, maxLines: 10)]
    public string description;

    [Header("Ingredients")]
    public List<RecipeIngredient> ingredientList = new List<RecipeIngredient>();
}

namespace Cooking
{
    [System.Serializable]
    public struct RecipeIngredient
    {
        public string name;
        public int quantity;
        public CookType cookType;
        public RipeState ripeState;
        public float targetVolume;

        public RecipeIngredient(string name, int quantity, CookType cookType, RipeState ripeState, float targetVolume)
        {
            this.name = name;
            this.quantity = quantity;
            this.cookType = cookType;
            this.ripeState = ripeState;
            this.targetVolume = targetVolume;
        }
    }
}