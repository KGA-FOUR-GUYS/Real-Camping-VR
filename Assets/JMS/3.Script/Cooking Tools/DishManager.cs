using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cooking;

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

    private void Start()
    {
        StartCoroutine(CollectIngredients(target));
    }

    private IEnumerator CollectIngredients(IngredientSO targetSO)
    {
        yield return new WaitForSeconds(5f);

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
