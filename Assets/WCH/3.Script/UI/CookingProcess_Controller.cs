using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookingProcess_Controller : MonoBehaviour
{
    public static CookingProcess_Controller instance = null;
    public Vector3 targetScale;

    private ScrollRect scrollRect;

    private RectTransform viewportRect;
    private RectTransform contentRect;

    public int currentIndex = 0;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        scrollRect = GetComponentInChildren<ScrollRect>();
    }


    private void FocusContent(int count, int A)
    {
        viewportRect = scrollRect.viewport;
        contentRect = scrollRect.content;
        float maxX = viewportRect.rect.width - contentRect.rect.width;
        float returnValue;

        if (A == 0 || A == count-1)
        {
            returnValue = A == 0 ? 0 : maxX;
        }
        else
        {

            Debug.Log(maxX);
            float divideX = 1f / (count - 1);

            float middleX = divideX * A;
            
            returnValue = Mathf.Lerp(0, maxX, middleX);
        }

        StartCoroutine(SmoothChange(new Vector2(returnValue, 0)));
    }
    public void ResetFocus()
    {
        currentIndex = 0;
        scrollRect.content.anchoredPosition = Vector2.zero;
    }
    public void PlusBtn()
    {
        if (currentIndex < scrollRect.content.childCount - 1) 
        { 
            currentIndex++;
            FocusContent(scrollRect.content.childCount, currentIndex);
            SoundManager.instance.PlayCookingSFX(0);
        }
        else if (currentIndex == scrollRect.content.childCount - 1)
        {
            RecipeManager.instance.ProcessChange(RecipeProcess.Result);
            SoundManager.instance.PlayCookingSFX(0);
        }
    }
    public void MinusBtn()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            FocusContent(scrollRect.content.childCount, currentIndex);
            SoundManager.instance.PlayCookingSFX(0);
        }
    }

    private IEnumerator SmoothChange(Vector2 returnValue)
    {
        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            StartCoroutine(RecipeManager.instance.Scaler(scrollRect.content.GetChild(i).transform, Vector3.one, targetScale,
                RecipeManager.instance.expandSpeed));
        }
        yield return new WaitForSeconds(RecipeManager.instance.expandSpeed);


        StartCoroutine(RecipeManager.instance.MoveSmooth(scrollRect.content, scrollRect.content.anchoredPosition, returnValue,
            RecipeManager.instance.moveSpeed));

        yield return new WaitForSeconds(RecipeManager.instance.moveSpeed);

        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            StartCoroutine(RecipeManager.instance.Scaler(scrollRect.content.GetChild(i).transform, targetScale, Vector3.one,
                RecipeManager.instance.expandSpeed));
        }

        yield break;
    }



}
