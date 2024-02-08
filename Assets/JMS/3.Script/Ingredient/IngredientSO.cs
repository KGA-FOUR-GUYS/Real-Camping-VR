using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cooking;

[CreateAssetMenu(fileName = "IngredientName", menuName = "ScriptableObjects/IngredientSO", order = 1)]
public class IngredientSO : ScriptableObject
{
    [Header("UI")]
    [Tooltip("�����ǿ� ���� UI �̹���")]
    public Sprite image;
    [Tooltip("�����ǿ� ���� ��� ����")]
    [TextArea(minLines: 3, maxLines: 10)]
    public string description;

    [Header("Materials")]
    [Tooltip("�� ��")]
    public Material rawMaterial;
    [Tooltip("�� ����")]
    public Material undercookMaterial;
    [Tooltip("�� ����")]
    public Material welldoneMaterial;
    [Tooltip("�ʹ� ����")]
    public Material overcookMaterial;
    [Tooltip("Ÿ����")]
    public Material burntMaterial;

    [Header("Cook Factor")]
    [Tooltip("�⺻ ����ġ\n  baseWeight * volumeWeight�� �ʹ� �ӵ� ����")]
    [Range(1f, 10f)] public float baseWeight = 1f;
    [Tooltip("�ּ� ����")]
    [Range(0.1f, 1000f)] public float minVolume = 1f;
    [Tooltip("���ǿ� ���� ����ġ\n  baseWeight * volumeWeight�� �ʹ� �ӵ� ����")]
    public AnimationCurve weightOverVolume;
}
