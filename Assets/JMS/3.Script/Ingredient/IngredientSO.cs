using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cooking;

[CreateAssetMenu(fileName = "IngredientName", menuName = "ScriptableObjects/IngredientSO", order = 1)]
public class IngredientSO : ScriptableObject
{
    [Header("UI")]
    public Sprite image;
    [TextArea(minLines: 3, maxLines: 10)]
    public string description;

    [Header("Raw")]
    public Material rawMaterial;

    [Header("Undercook")]
    public float ripeForUndercook = 100f;
    public Material undercookMaterial;

    [Header("Welldone")]
    public float ripeForWelldone = 200f;
    public Material welldoneMaterial;

    [Header("Overcook")]
    public float ripeForOvercook = 300f;
    public Material overcookMaterial;

    [Header("Burn")]
    public float ripeForBurn = 400f;
    public Material burnMaterial;

    [Header("Weight")]
    [Tooltip("�⺻ ����ġ\n  baseWeight * volumeWeight�� �ʹ� �ӵ� ����")]
    [Range(1f, 10f)] public float baseWeight = 1f;
    [Tooltip("���ǿ� ���� ����ġ\n  baseWeight * volumeWeight�� �ʹ� �ӵ� ����")]
    public AnimationCurve weightOverVolume;

    [Header("Optimization")]
    [Range(0.1f, 1000f)] public float minVolume = 1f;
}
