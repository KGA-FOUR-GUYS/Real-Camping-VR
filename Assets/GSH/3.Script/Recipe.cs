using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class Recipe
{
    [Tooltip("레시피 ID")]
    public int id;
    [Tooltip("레시피 이름")]
    public string name;
    [Tooltip("과정")]
    public string[] progress;
    [Tooltip("이미지")]
    public Image[] image;
    [Tooltip("사용 도구")]
    public string[] utensils;
    [Tooltip("사용 기능")]
    public string[] function;
    [Tooltip("필요한 재료")]
    public string[] materials;
}
[System.Serializable]
public class RecipeEvent
{
    public int ID;
    public string Name;

    public Vector2 line;
    public Recipe[] progress;
}
