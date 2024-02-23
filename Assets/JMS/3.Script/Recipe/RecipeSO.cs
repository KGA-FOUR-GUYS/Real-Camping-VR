using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cooking;

[CreateAssetMenu(fileName = "RecipeName", menuName = "ScriptableObjects/RecipeSO", order = 0)]
public class RecipeSO : ScriptableObject
{
    [Header("UI")]
    public int id;
    [Tooltip("������ UI �̹���")]
    public Sprite image;
    [Tooltip("������ ����")]
    [TextArea(minLines: 3, maxLines: 10)]
    public string description;
    [Tooltip("������ �䱸 ����")]
    public int requiredLevel;
    [Tooltip("������ ���� ����ġ")]
    public int rewardExp;
    [Tooltip("������ ���� ��")]
    public int rewardMoney;

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
        public float ripeByBroil;
        public float ripeByBoil;
        public float ripeByGrill;
        public RipeState ripeState;
        public float sliceVolume;
        public float sliceCount;

        public RecipeIngredient(string name, int quantity, float ripeByBroil, float ripeByBoil, float ripeByGrill, RipeState ripeState, float targetVolume, int targetCount)
        {
            this.name = name;
            this.quantity = quantity;
            this.ripeByBroil = ripeByBroil;
            this.ripeByBoil = ripeByBoil;
            this.ripeByGrill = ripeByGrill;
            this.ripeState = ripeState;
            this.sliceVolume = targetVolume;
            this.sliceCount = targetCount;
        }
    }
}