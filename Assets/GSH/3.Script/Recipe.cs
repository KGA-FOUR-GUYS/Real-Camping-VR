using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class Recipe
{
    [Tooltip("������ ID")]
    public int id;
    [Tooltip("������ �̸�")]
    public string name;
    [Tooltip("����")]
    public string[] progress;
    [Tooltip("�̹���")]
    public Image[] image;
    [Tooltip("��� ����")]
    public string[] utensils;
    [Tooltip("��� ���")]
    public string[] function;
    [Tooltip("�ʿ��� ���")]
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
