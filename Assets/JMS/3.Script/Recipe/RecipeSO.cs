using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cooking;

[CreateAssetMenu(fileName = "RecipeName", menuName = "ScriptableObjects/RecipeSO", order = 0)]
public class RecipeSO : ScriptableObject
{
    [Header("UI")]
    public int id;
    [Tooltip("레시피 UI 이미지")]
    public Sprite image;
    [Tooltip("레시피 설명")]
    [TextArea(minLines: 3, maxLines: 10)]
    public string description;
    [Tooltip("레시피 요구 레벨")]
    public int requiredLevel;
    [Tooltip("레시피 보상 경험치")]
    public int rewardExp;
    [Tooltip("레시피 보상 돈")]
    public int rewardMoney;

    [Header("Ingredients")]
    public List<RecipeIngredient> ingredientList = new List<RecipeIngredient>();

    public List<CookingProcess> GetProcessList()
    {
        var processKVP = new Dictionary<CookingProcess, int>();
        foreach (var ingredient in ingredientList)
        {
            if (!processKVP.ContainsKey(CookingProcess.Boil)
                && ingredient.ripeByBoil > 0)
            {
                processKVP.Add(CookingProcess.Boil, (int)CookingProcess.Boil);
            }

            if (!processKVP.ContainsKey(CookingProcess.Broil)
                && ingredient.ripeByBroil > 0)
            {
                processKVP.Add(CookingProcess.Broil, (int)CookingProcess.Broil);
            }

            if (!processKVP.ContainsKey(CookingProcess.Grill)
                && ingredient.ripeByGrill > 0)
            {
                processKVP.Add(CookingProcess.Grill, (int)CookingProcess.Grill);
            }
        }

        var processList = new List<CookingProcess>();
        foreach (var key in processKVP.Keys)
        {
            processList.Add(key);
        }

        return processList;
    }
}

namespace Cooking
{
    [System.Serializable]
    public struct RecipeIngredient
    {
        public string name;
        public int quantity;
        public float ripeByBroil;
        public float ripeByBoil;
        public float ripeByGrill;
        public RipeState ripeState;
        public float sliceVolume;
        public float sliceCount;

        public RecipeIngredient(string name, int quantity, float ripeByBroil, float ripeByBoil, float ripeByGrill, RipeState ripeState, float sliceVolume, int sliceCount)
        {
            this.name = name;
            this.quantity = quantity;
            this.ripeByBroil = ripeByBroil;
            this.ripeByBoil = ripeByBoil;
            this.ripeByGrill = ripeByGrill;
            this.ripeState = ripeState;
            this.sliceVolume = sliceVolume;
            this.sliceCount = sliceCount;
        }
    }
}