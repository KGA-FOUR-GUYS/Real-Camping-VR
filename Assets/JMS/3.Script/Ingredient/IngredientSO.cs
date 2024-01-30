using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cooking;

[CreateAssetMenu(fileName = "IngredientName", menuName = "ScriptableObjects/IngredientSO", order = 1)]
public class IngredientSO : ScriptableObject
{
    [Tooltip("�����ǿ� ���� UI �̹���")]
    public Sprite image;
    [Tooltip("�����ǿ� ���� ��� ����")]
    [TextArea(minLines: 3, maxLines: 10)]
    public string description;

    [Header("Prefab Model")]
    [Tooltip("�� ��")]
    public GameObject raw;
    [Tooltip("�� ����")]
    public GameObject undercook;
    [Tooltip("�� ����")]
    public GameObject welldone;
    [Tooltip("�ʹ� ����")]
    public GameObject overcook;
    [Tooltip("Ÿ����")]
    public GameObject burnt;

    [Header("Cook Factor")]
    [Tooltip("�⺻ ����ġ\n  baseWeight * volumeWeight�� �ʹ� �ӵ� ����")]
    [Range(1f, 10f)] public float baseCookWeight = 1.0f;
    [Tooltip("�ּ� ����")]
    [Range(10f, 1000f)] public float minVolume = 10f;
    [Tooltip("���ǿ� ���� ����ġ\n  baseWeight * volumeWeight�� �ʹ� �ӵ� ����")]
    public AnimationCurve weightOverVolume;
}
