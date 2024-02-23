using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum RecipeProcess
{
    SelectRecipe = 1,
    DetailRecipe,
    CookingProcess,
    Result=99
}
public class RecipeManager : MonoBehaviour
{
    public static RecipeManager instance = null;
    //----inspector----
    public float canvasFadeSpeed = 0.2f;
    public float moveSpeed = 0.2f;
    public float expandSpeed = 0.2f;

    public RecipeSO currentSO;

    //----inspector----

    public Recipe_UI recipe_UI = null;
    private bool isCoroutine = false;
    private RecipeProcess currentProcess;

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
    [SerializeField] private Transform detailTextParent;
    [SerializeField] private GameObject detailTextPrefab;

    [SerializeField] private GameObject CookingProcessCanvas;
    [SerializeField] private GameObject CookingProcessTopSpace;
    [SerializeField] private Vector3 targetVector_CP;

    [SerializeField] private GameObject ResultCanvas;
    [SerializeField] private Transform ResultImgParent;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            currentProcess = RecipeProcess.SelectRecipe;
            currentCanvas = RecipeSelectCanvas;
            textOriginPos = titleText.rectTransform.localPosition;
        }
        else Destroy(gameObject);
    }


    public void ProcessChange(RecipeProcess process, string text = null)
    {
        if (isCoroutine) return;

        if(process.Equals(RecipeProcess.SelectRecipe))
        {
            currentProcess = RecipeProcess.SelectRecipe;

            StartCoroutine(CanvasAlphaChange(currentCanvas, canvasFadeSpeed, 1f, 0f));
            currentCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            currentCanvas = RecipeSelectCanvas;
            currentCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
            StartCoroutine(CanvasAlphaChange(currentCanvas, canvasFadeSpeed, 0f, 1f));

            StartCoroutine(MoveOutText());
            StartCoroutine(MoveInText("< Recipe List >"));


        }

        else if (process.Equals(RecipeProcess.DetailRecipe))
        {
            DetailTextChange();
            if (currentProcess.Equals(RecipeProcess.CookingProcess))
            {
                StartCoroutine(TopSpaceMove(CookingProcessTopSpace.transform, targetVector_CP, canvasFadeSpeed));
            }
            else
            {
                StartCoroutine(MoveImgAndExpand());
                StartCoroutine(MoveOutText());
                StartCoroutine(MoveInText(string.Format($"< {text} >")));
            }
            currentProcess = RecipeProcess.DetailRecipe;

            StartCoroutine(CanvasAlphaChange(currentCanvas, canvasFadeSpeed, 1f, 0f));
            currentCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            currentCanvas = RecipeDetailCanvas;
            currentCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
            StartCoroutine(CanvasAlphaChange(currentCanvas, canvasFadeSpeed, 0f, 1f));

        }

        else if (process.Equals(RecipeProcess.CookingProcess))
        {
            currentProcess = RecipeProcess.CookingProcess;
            CookingProcess_Controller.instance.ResetFocus();

            StartCoroutine(CanvasAlphaChange(currentCanvas, canvasFadeSpeed, 1f, 0f));
            currentCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            currentCanvas = CookingProcessCanvas;
            currentCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
            StartCoroutine(CanvasAlphaChange(currentCanvas, canvasFadeSpeed, 0f, 1f));

            StartCoroutine(TopSpaceMove(CookingProcessTopSpace.transform, Vector3.zero, moveSpeed));
        }

        else if (process.Equals(RecipeProcess.Result))
        {
            currentProcess = RecipeProcess.Result;

            StartCoroutine(CanvasAlphaChange(currentCanvas, canvasFadeSpeed, 1f, 0f));
            currentCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            currentCanvas = ResultCanvas;
            currentCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
            StartCoroutine(CanvasAlphaChange(currentCanvas, canvasFadeSpeed, 0f, 1f));
            cookingImgObj.transform.parent = ResultImgParent;
        }
    }

    private void ActiveToggle(GameObject origin)
    {
        currentCanvas.SetActive(false);
        currentCanvas = origin;
        currentCanvas.SetActive(true);
    }
    public void StartCookBtn()
    {
        ProcessChange(RecipeProcess.CookingProcess);
    }
    public void StopCookBtn()
    {
        ProcessChange(RecipeProcess.DetailRecipe);
    }

    public void BackBtn()
    {
        if (currentProcess.Equals(RecipeProcess.SelectRecipe))
        {

        }
        else if (currentProcess.Equals(RecipeProcess.DetailRecipe))
        {
            recipe_UI = null;
            ProcessChange(RecipeProcess.SelectRecipe);
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
        
        cookingImgObj = Instantiate(recipe_UI.instantiateObj);
        cookingImgObj.transform.parent = instantiateParent;
        cookingImgObj.transform.position = recipe_UI.instantiateObj.transform.position;
        cookingImgObj.transform.localScale = recipe_UI.instantiateObj.transform.localScale;
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
        //StartCoroutine(CanvasAlphaChange(currentCanvas, canvasFadeSpeed, 0f, 1f));
        //yield return new WaitForSeconds(canvasFadeSpeed);
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

    //-----------------------------ScrollRect---------------------------------
    public IEnumerator Scaler(Transform transform, Vector3 origin, Vector3 target, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            transform.localScale = Vector3.Lerp(origin, target, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield break;
    }
    public IEnumerator MoveSmooth(RectTransform transform, Vector3 origin, Vector3 target, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            transform.anchoredPosition = Vector3.Lerp(origin, target, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield break;
    }

    private IEnumerator TopSpaceMove(Transform target, Vector3 targetVector, float time)
    {
        float elapsedTime = 0f;
        Vector3 origin = target.localPosition;
        while (elapsedTime < time)
        {
            target.localPosition = Vector3.Lerp(origin, targetVector, elapsedTime / time);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        target.localPosition = targetVector;

        yield break;
    }
    //-----------------------------ScrollRect---------------------------------



    //------------------------------Detail-------------------------------------
    private void DetailTextChange()
    {
        if (detailTextParent.childCount > 0)
        {
            Destroy(detailTextParent.GetChild(0).gameObject);
        }

        GameObject instantiateText = Instantiate(detailTextPrefab, detailTextParent);

        instantiateText.GetComponent<TextMeshProUGUI>().text = currentSO.description;
    }



    //------------------------------Detail-------------------------------------
    


    
    //-----------------------------Result---------------------------------
    public void ExitBtn()
    {
        ProcessChange(RecipeProcess.SelectRecipe);
        CookingProcessTopSpace.transform.position = targetVector_CP;
        cookingImgObj = null;
    }

    //-----------------------------Result---------------------------------



}
