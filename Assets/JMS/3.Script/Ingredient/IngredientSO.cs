using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cooking;

[CreateAssetMenu(fileName = "IngredientName", menuName = "ScriptableObjects/IngredientSO", order = 1)]
public class IngredientSO : ScriptableObject
{
    [Tooltip("레시피에 보일 UI 이미지")]
    public Sprite image;
    [Tooltip("레시피에 보일 재료 설명")]
    [TextArea(minLines: 3, maxLines: 10)]
    public string description;

    [Header("Prefab Model")]
    [Tooltip("날 것")]
    public GameObject raw;
    [Tooltip("덜 익음")]
    public GameObject undercook;
    [Tooltip("잘 익음")]
    public GameObject welldone;
    [Tooltip("너무 익음")]
    public GameObject overcook;
    [Tooltip("타버림")]
    public GameObject burnt;

    [Header("Cook Factor")]
    [Tooltip("기본 가중치\n  baseWeight * volumeWeight로 익는 속도 조절")]
    [Range(1f, 10f)] public float baseCookWeight = 1.0f;
    [Tooltip("최소 부피")]
    [Range(10f, 1000f)] public float minVolume = 10f;
    [Tooltip("부피에 따른 가중치\n  baseWeight * volumeWeight로 익는 속도 조절")]
    public AnimationCurve weightOverVolume;
}
