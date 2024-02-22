using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI_Manager : MonoBehaviour
{
    public static LobbyUI_Manager instance = null;

    public float moveTime = 0.2f;

    private bool isCoroutine = false;

    [SerializeField] private RectTransform maskRect;

    [SerializeField] private RectTransform home_Rect;
    [SerializeField] private RectTransform map_Rect;
    [SerializeField] private RectTransform collection_Rect;
    [SerializeField] private RectTransform setting_Rect;

    private ColorBlock originCB;
    private ColorBlock SelectedCB;

    private Color normalColor = new Color(100f / 255f, 200f / 255f, 1f);
    private Color pressedColor = new Color(100f / 255f, 120f / 255f, 1f);
    [SerializeField] private Button home_Btn;
    [SerializeField] private Button map_Btn;
    [SerializeField] private Button collection_Btn;
    [SerializeField] private Button setting_Btn;

    [SerializeField] private RectTransform map_Img_Rect;
    [SerializeField] private GameObject map_Detail_GO;
    [SerializeField] private RectTransform pinPoint_Content;
    [SerializeField] private RectTransform center;
    private bool isDetailCo = false;
    private bool isDetail = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ChangeActive(home_Rect);
        originCB = home_Btn.colors;

        SelectedCB = originCB;
        SelectedCB.normalColor = normalColor;
        SelectedCB.highlightedColor = normalColor;
        SelectedCB.selectedColor = normalColor;
        SelectedCB.pressedColor = pressedColor;

    }

    private IEnumerator MaskSizeChange(RectTransform rect, float time)
    {
        isCoroutine = true;
        float elapsedTime = 0f;

        float originX = rect.rect.width;
        float originY = rect.rect.height;
        
        Vector2 targetV2 = new Vector2(originX, 10f);
        Vector2 startV2 = new Vector2(0, 10f);

        while (elapsedTime < time)
        {
            rect.sizeDelta = Vector2.Lerp(startV2, targetV2, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rect.sizeDelta = targetV2;

        elapsedTime = 0f;
        startV2 = targetV2;
        targetV2 = new Vector2(originX, originY);

        while (elapsedTime < time)
        {
            rect.sizeDelta = Vector2.Lerp(startV2, targetV2, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rect.sizeDelta = targetV2;

        isCoroutine = false;

        yield break;
    }

    private void ChangeActive(RectTransform rect)
    {
        home_Rect.gameObject.SetActive(false);
        map_Rect.gameObject.SetActive(false);
        collection_Rect.gameObject.SetActive(false);
        setting_Rect.gameObject.SetActive(false);

        rect.gameObject.SetActive(true);
    }

    public void HomeBtn()
    {
        if (isCoroutine)
        {
            return;
        }
        ChangeActive(home_Rect);
        ChangeBtnColor(home_Btn);
        Debug.Log("Home");
        StartCoroutine(MaskSizeChange(maskRect, moveTime));
        //StartCoroutine(ScaleChange(home_Rect, moveTime));
    }
    public void MapBtn()
    {
        if (isCoroutine)
        {
            return;
        }
        ChangeActive(map_Rect);
        ChangeBtnColor(map_Btn);
        Debug.Log("Map");
        StartCoroutine(MaskSizeChange(maskRect, moveTime));
        StartCoroutine(MoveDetail(false, moveTime));
        map_Img_Rect.localPosition = Vector3.zero;
    }
    public void CollectionBtn()
    {
        if (isCoroutine)
        {
            return;
        }
        ChangeActive(collection_Rect);
        ChangeBtnColor(collection_Btn);
        Debug.Log("Collection");
        StartCoroutine(MaskSizeChange(maskRect, moveTime));
    }
    public void SettingBtn()
    {
        if (isCoroutine)
        {
            return;
        }
        ChangeActive(setting_Rect);
        ChangeBtnColor(setting_Btn);
        Debug.Log("Setting");
        StartCoroutine(MaskSizeChange(maskRect, moveTime));
    }


    public void FocusMapPoint(RectTransform myRect)
    {
        if (isCoroutine)
        {
            return;
        }
        Vector3 difference = center.position - myRect.position;
        StartCoroutine(MovetoCenter(map_Img_Rect, difference, moveTime));
        StartCoroutine(MoveDetail(true, moveTime));
    }
    public void CancelFocus()
    {
        StartCoroutine(MoveDetail(false, moveTime));
    }

    private void ChangeBtnColor(Button currentBtn)
    {
        
        home_Btn.colors = originCB;
        map_Btn.colors = originCB;
        collection_Btn.colors = originCB;
        setting_Btn.colors = originCB;

        currentBtn.colors = SelectedCB;
    }



    private IEnumerator MovetoCenter(RectTransform origin, Vector3 difference, float time)
    {
        isCoroutine = true;
        float elapsedTime = 0f;

        Vector3 targetV3 = origin.position + difference;

        while (elapsedTime < time)
        {
            origin.position = Vector3.Lerp(origin.position, targetV3, elapsedTime / time);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        origin.position = targetV3;
        isCoroutine = false;
        yield break;
    }

    private IEnumerator MoveDetail(bool isOn, float time)
    {
        if (isDetailCo)
        {
            yield break;
        }
        isDetailCo = true;
        Vector3 origin = map_Detail_GO.transform.localPosition;

        float elapsedTime = 0f;
        if (isOn && !isDetail)
        {
            Vector3 targetV3 = origin + new Vector3(600, 0, 0);
            isDetail = true;
            while (elapsedTime < time)
            {
                map_Detail_GO.transform.localPosition = Vector3.Lerp(origin, targetV3, elapsedTime / time);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            map_Detail_GO.transform.localPosition = targetV3;
        }

        else if (!isOn && isDetail)
        {
            Vector3 targetV3 = origin - new Vector3(600, 0, 0);
            isDetail = false;
            while (elapsedTime < time)
            {
                map_Detail_GO.transform.localPosition = Vector3.Lerp(origin, targetV3, elapsedTime / time);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            map_Detail_GO.transform.localPosition = targetV3;
        }

        isDetailCo = false;
        yield break;
    }

}
