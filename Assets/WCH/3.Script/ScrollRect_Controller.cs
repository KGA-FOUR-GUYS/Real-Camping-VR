using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRect_Controller : MonoBehaviour
{
    public static ScrollRect_Controller instance = null;
    private ScrollRect scrollRect;

    private RectTransform viewportRect;
    private RectTransform contentRect;

    [SerializeField] private int currentIndex = 0;
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

        //contentRect.anchoredPosition = new Vector2(returnValue, 0);
        StartCoroutine(SmoothChange(new Vector2(returnValue, 0)));
    }

    public void PlusBtn()
    {
        if (currentIndex < scrollRect.content.childCount - 1) 
        { 
            currentIndex++;
            FocusContent(scrollRect.content.childCount, currentIndex);
        }
    }
    public void MinusBtn()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            FocusContent(scrollRect.content.childCount, currentIndex);
        }
    }

    private IEnumerator SmoothChange(Vector2 returnValue)
    {
        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            StartCoroutine(RecipeManager.instance.Scaler(scrollRect.content.GetChild(i).transform, Vector3.one, new Vector3(0.9f,0.9f,0.9f),
                RecipeManager.instance.expandSpeed));
        }
        yield return new WaitForSeconds(RecipeManager.instance.expandSpeed);


        StartCoroutine(RecipeManager.instance.MoveSmooth(scrollRect.content, scrollRect.content.anchoredPosition, returnValue,
            RecipeManager.instance.moveSpeed));

        yield return new WaitForSeconds(RecipeManager.instance.moveSpeed);

        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            StartCoroutine(RecipeManager.instance.Scaler(scrollRect.content.GetChild(i).transform, new Vector3(0.9f, 0.9f, 0.9f), Vector3.one,
                RecipeManager.instance.expandSpeed));
        }

        yield break;
    }

}
