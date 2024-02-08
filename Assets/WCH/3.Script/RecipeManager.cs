using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum CookingProcess
{
    SelectRecipe = 1,
    DetailRecipe
}
public class RecipeManager : MonoBehaviour
{
    public static RecipeManager instance = null;

    //----inspector----
    public float canvasFadeSpeed = 0.2f;
    public float moveSpeed = 0.2f;
    public float expandSpeed = 0.2f;
    //----inspector----

    public UIAction uiAction = null;
    private bool isCoroutine = false;
    private CookingProcess currentProcess;

    //----Instantiate----
    [SerializeField] private Transform instantiateParent;
    [SerializeField] private Transform standardSize;
    private GameObject cookingImgObj = null;
    //----Instantiate----

    private GameObject currentCanvas;
    [SerializeField] private TextMeshProUGUI titleText;
    private Vector3 textOriginPos;
    [SerializeField] private GameObject RecipeSelectCanvas;
    [SerializeField] private GameObject RecipeDetailCanvas;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            currentProcess = CookingProcess.SelectRecipe;
            currentCanvas = RecipeSelectCanvas;
            textOriginPos = titleText.rectTransform.localPosition;
        }
        else Destroy(gameObject);
    }


    public void ProcessChange(CookingProcess process, string text = null)
    {
        if (isCoroutine) return;

        if(process.Equals(CookingProcess.SelectRecipe))
        {
            currentProcess = CookingProcess.SelectRecipe;

            StartCoroutine(CanvasAlphaChange(currentCanvas, canvasFadeSpeed, 1f, 0f));
            //ActiveToggle(RecipeSelectCanvas);
            currentCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            currentCanvas = RecipeSelectCanvas;
            currentCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
            StartCoroutine(CanvasAlphaChange(currentCanvas, canvasFadeSpeed, 0f, 1f));
            StartCoroutine(MoveOutText());
            //titleText.text = "< Recipe List >";
            StartCoroutine(MoveInText("< Recipe List >"));


        }

        else if (process.Equals(CookingProcess.DetailRecipe))
        {
            currentProcess = CookingProcess.DetailRecipe;

            StartCoroutine(CanvasAlphaChange(currentCanvas, canvasFadeSpeed, 1f, 0f));
            //ActiveToggle(RecipeDetailCanvas);
            currentCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            currentCanvas = RecipeDetailCanvas;
            currentCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;

            StartCoroutine(MoveImgAndExpand());
            //StartCoroutine(CanvasAlphaChange(currentCanvas, 0.2f, 0f, 1f));
            StartCoroutine(MoveOutText());
            //titleText.text = string.Format($"< {text} >");
            StartCoroutine(MoveInText(string.Format($"< {text} >")));

        }
    }

    private void ActiveToggle(GameObject origin)
    {
        currentCanvas.SetActive(false);
        currentCanvas = origin;
        currentCanvas.SetActive(true);
    }

    public void BackBtn()
    {
        if (currentProcess.Equals(CookingProcess.SelectRecipe))
        {

        }
        else if (currentProcess.Equals(CookingProcess.DetailRecipe))
        {
            uiAction = null;
            ProcessChange(CookingProcess.SelectRecipe);
        }
    }



    /// <summary>
    /// Canvas must have CanvasGroup
    /// </summary>
    /// <param name="canvas">1</param>
    /// <param name="time"></param>
    /// <param name="startAlpha"></param>
    /// <param name="endAlpha"></param>
    /// <returns></returns>
    private IEnumerator CanvasAlphaChange(GameObject canvas, float time, float startAlpha, float endAlpha)
    {
        isCoroutine = true;
        time = (time < 0) ? 0 : time;
        startAlpha = Mathf.Clamp01(startAlpha);
        endAlpha = Mathf.Clamp01(endAlpha);

        float elapsedTime = 0f;

        CanvasGroup alpha = canvas.GetComponent<CanvasGroup>();

        while (elapsedTime < time)
        {
            alpha.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / time);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        alpha.alpha = endAlpha;

        isCoroutine = false;
        yield break;

    }
    private void CreateImg()
    {
        if(cookingImgObj != null)
        {
            Destroy(cookingImgObj);
        }

        cookingImgObj = Instantiate(uiAction.instantiateObj);
        cookingImgObj.transform.parent = instantiateParent;
        cookingImgObj.transform.position = uiAction.instantiateObj.transform.position;
        cookingImgObj.transform.localScale = uiAction.instantiateObj.transform.localScale;
        cookingImgObj.AddComponent<CanvasGroup>().ignoreParentGroups = true;
    }

    private IEnumerator MoveImgAndExpand()
    {
        isCoroutine = true;
        CreateImg();
        Vector3 originPos = cookingImgObj.transform.position;
        float elapsedTime = 0f;
        while (elapsedTime < moveSpeed)
        {
            cookingImgObj.transform.position = Vector3.Lerp(originPos, instantiateParent.position, elapsedTime / moveSpeed);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        cookingImgObj.transform.position = instantiateParent.position;
        elapsedTime = 0f;
        while (elapsedTime < expandSpeed)
        {
            cookingImgObj.transform.localScale = Vector3.Lerp(Vector3.one, standardSize.localScale, elapsedTime / expandSpeed);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        cookingImgObj.transform.localScale = standardSize.localScale;
        StartCoroutine(CanvasAlphaChange(currentCanvas, canvasFadeSpeed, 0f, 1f));
        yield return new WaitForSeconds(canvasFadeSpeed);
        cookingImgObj.GetComponent<CanvasGroup>().ignoreParentGroups = false;

        //if !CanvasAlphaChange - isCoroutine = false;
        yield break;
    }

    private IEnumerator MoveOutText()
    {
        float elapsedTime = 0f;
        Vector3 targetPos = textOriginPos + new Vector3(-500, 0, 0);
        while (elapsedTime < canvasFadeSpeed)
        {
            titleText.rectTransform.localPosition = Vector3.Lerp(textOriginPos, targetPos, elapsedTime / canvasFadeSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield break;
    }
    private IEnumerator MoveInText(string text)
    {
        yield return new WaitForSeconds(canvasFadeSpeed);
        float elapsedTime = 0f;
        titleText.text = text;

        Vector3 movePos = textOriginPos + new Vector3(500, 0, 0);
        while (elapsedTime < canvasFadeSpeed)
        {
            titleText.rectTransform.localPosition = Vector3.Lerp(movePos, textOriginPos, elapsedTime / canvasFadeSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        titleText.rectTransform.localPosition = textOriginPos;

        yield break;
    }
}
