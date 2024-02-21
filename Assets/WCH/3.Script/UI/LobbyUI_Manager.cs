using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI_Manager : MonoBehaviour
{
    public static LobbyUI_Manager instance = null;

    public float moveTime = 0.2f;

    private bool isCoroutine = false;

    [SerializeField] private GameObject Screen_Home;
    [SerializeField] private GameObject Screen_Map;
    [SerializeField] private GameObject Screen_Collection;
    [SerializeField] private GameObject Screen_Setting;

    private RectTransform home_Rect;
    private RectTransform map_Rect;
    private RectTransform collection_Rect;
    private RectTransform setting_Rect;

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
        home_Rect = Screen_Home.GetComponent<RectTransform>();
        map_Rect = Screen_Map.GetComponent<RectTransform>();
        //collection_Rect = Screen_Collection.GetComponent<RectTransform>();
        //setting_Rect = Screen_Setting.GetComponent<RectTransform>();

        ChangeActive(home_Rect);
    }

    private void ChangeBanner(RectTransform rect)
    {

    }

    private IEnumerator ScaleChange(RectTransform rect, float time)
    {
        isCoroutine = true;

        float elapsedTime = 0f;
        rect.localScale = Vector3.zero;
        Vector3 origin = rect.localScale + new Vector3(0, 0.01f, 1);
        while (elapsedTime < time)
        {
            rect.localScale = Vector3.Lerp(origin, new Vector3(1, 0.01f, 1), elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0f;
        origin = new Vector3(1, 0.1f, 1);
        while (elapsedTime < time)
        {
            rect.localScale = Vector3.Lerp(origin, Vector3.one, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        rect.localScale = Vector3.one;

        isCoroutine = false;
        yield break;
    }


    //private IEnumerator RectMoveSmooth(RectTransform rect,Vector3 targetVector , float time)
    //{
    //    float elapsedTime = 0f;
    //    Vector3 origin = rect.localPosition;
    //    while (elapsedTime < time)
    //    {
    //        rect.position = Vector3.Lerp(origin, targetVector, elapsedTime / time);
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }
    //    rect.localPosition = targetVector;

    //    yield break;
    //}

    public void FocusMapPoint()
    {

    }
    private void ChangeActive(RectTransform rect)
    {
        home_Rect.gameObject.SetActive(false);
        map_Rect.gameObject.SetActive(false);
        //todo - screen 만들어지는대로 넣기
        //home_Rect.gameObject.SetActive(false);
        //home_Rect.gameObject.SetActive(false);

        rect.gameObject.SetActive(true);
    }

    public void HomeBtn()
    {
        if (isCoroutine)
        {
            return;
        }
        ChangeActive(home_Rect);
        Debug.Log("Home");
        StartCoroutine(ScaleChange(home_Rect, moveTime));
    }
    public void MapBtn()
    {
        ChangeActive(map_Rect);
        Debug.Log("Map");
        StartCoroutine(ScaleChange(map_Rect, moveTime));

    }
    public void CollectionBtn()
    {
        Debug.Log("Collection");

    }
    public void SettingBtn()
    {
        Debug.Log("Setting");

    }

}
