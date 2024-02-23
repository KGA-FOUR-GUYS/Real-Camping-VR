using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cooking;

[RequireComponent(typeof(SphereCollider))]
public class DishManager : MonoBehaviour
{
    [Header("Collect ingredients")]
    public IngredientSO target;
    public float duration = 3f;
    public float midOffsetY = 10f;
    public float endOffsetY = 2f;
    public float randomRange = .5f;
    public AnimationCurve positionOverTime;
    public float collectInterval = .1f;
    public float parentingDelay = .5f;

    [Header("Toggle Area")]
    [Range(2f, 10f)] public float timeToDisable = 3f;
    private SphereCollider dishAreaCollider;

    private void Start()
    {
        TryGetComponent(out dishAreaCollider);

        StartCoroutine(CollectIngredients(target));
    }

    private void Update()
    {
        var isUpward = Vector3.Dot(transform.up, Vector3.up) > 0;

        // ���� �� �ִ� ��Ȳ�̸�
        if (_currentCheckFlipedTime != null && isUpward)
        {
            StopCoroutine(_currentCheckFlipedTime);
            _currentCheckFlipedTime = null;
            dishAreaCollider.enabled = true;
        }
        // ���� �� ���� ��Ȳ�̸�
        if (_currentCheckFlipedTime == null && !isUpward)
        {
            _currentCheckFlipedTime = CheckFlipedTime();
            StartCoroutine(_currentCheckFlipedTime);
        }
    }

    private IEnumerator _currentCheckFlipedTime = null;
    private IEnumerator CheckFlipedTime()
    {
        float elapsedTime = 0f;
        while (elapsedTime < timeToDisable)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        dishAreaCollider.enabled = false;
    }

    private IEnumerator CollectIngredients(IngredientSO targetSO)
    {
        yield return new WaitForSeconds(1f);

        var ingredients = FindObjectsOfType<IngredientManager>();
        var targets = new List<IngredientManager>();
        foreach (var ingredient in ingredients)
        {
            if (ingredient.data != null && ingredient.data.Equals(targetSO))
            {
                targets.Add(ingredient);
            }
        }

        foreach (var target in targets)
        {
            target.StartCoroutine(target.FlyToDish(transform, midOffsetY, endOffsetY, randomRange, duration, positionOverTime));
            yield return new WaitForSeconds(collectInterval);
        }
    }
}