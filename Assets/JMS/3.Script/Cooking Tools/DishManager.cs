using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cooking;

[RequireComponent(typeof(SphereCollider))]
public class DishManager : MonoBehaviour
{
    public List<IngredientSO> targetIngredients = new List<IngredientSO>();

    [Header("Collect ingredients")]
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

        StartCoroutine(CollectIngredients(targetIngredients));
    }

    private void Update()
    {
        var isUpward = Vector3.Dot(transform.up, Vector3.up) > 0;

        // 담을 수 있는 상황이면
        if (_currentCheckFlipedTime != null && isUpward)
        {
            StopCoroutine(_currentCheckFlipedTime);
            _currentCheckFlipedTime = null;
            dishAreaCollider.enabled = true;
        }
        // 담을 수 없는 상황이면
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

    private IEnumerator CollectIngredients(List<IngredientSO> targetList)
    {
        yield return new WaitForSeconds(1f);

        var ingredients = FindObjectsOfType<IngredientDataManager>();
        var targets = new List<IngredientDataManager>();
        foreach (var ingredient in ingredients)
        {
			foreach (var target in targetList)
			{
                if (ingredient.data != null && ingredient.data.Equals(target))
                {
                    targets.Add(ingredient);
                }
            }
        }

        foreach (var target in targets)
        {
            target.StartCoroutine(target.FlyToDish(transform, midOffsetY, endOffsetY, randomRange, duration, positionOverTime));
            yield return new WaitForSeconds(collectInterval);
        }
    }
}
