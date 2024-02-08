using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cooking;

[CreateAssetMenu(fileName = "IngredientName", menuName = "ScriptableObjects/IngredientSO", order = 1)]
public class IngredientSO : ScriptableObject
{
    [Header("UI")]
    [Tooltip("레시피에 보일 UI 이미지")]
    public Sprite image;
    [Tooltip("레시피에 보일 재료 설명")]
    [TextArea(minLines: 3, maxLines: 10)]
    public string description;

    [Header("Materials")]
    [Tooltip("날 것")]
    public Material rawMaterial;
    [Tooltip("덜 익음")]
    public Material undercookMaterial;
    [Tooltip("잘 익음")]
    public Material welldoneMaterial;
    [Tooltip("너무 익음")]
    public Material overcookMaterial;
    [Tooltip("타버림")]
    public Material burntMaterial;

    [Header("Cook Factor")]
    [Tooltip("기본 가중치\n  baseWeight * volumeWeight로 익는 속도 조절")]
    [Range(1f, 10f)] public float baseWeight = 1f;
    [Tooltip("최소 부피")]
    [Range(0.1f, 1000f)] public float minVolume = 1f;
    [Tooltip("부피에 따른 가중치\n  baseWeight * volumeWeight로 익는 속도 조절")]
    public AnimationCurve weightOverVolume;
}
