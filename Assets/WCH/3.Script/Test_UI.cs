using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Test_UI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public TextMeshProUGUI btnText;
    public Canvas canvas;
    public CanvasGroup canGroup;
    public Camera mainCamera;
    private float angle;

    public float directionAngle = 45f;

    private bool isOnUI = false;
    private bool isLook = true;
    private void Awake()
    {
        mainCamera = Camera.main.GetComponent<Camera>();
        canGroup = canvas.GetComponent<CanvasGroup>();
    }
    private void Update()
    {
        angle = AngleCalculator(mainCamera.transform, transform);
        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);
        if (angle < directionAngle && distance < 5f)
        {
            LookPlayer_UI();
            Debug.Log("directionAngle범위 안");
            if (isOnUI == false)
            {
                StartCoroutine(canvasFadeIn());
            }
        }
        else
        {
            isOnUI = false;
            canGroup.alpha = 0;
            Debug.Log("범위 밖");
        }
    }

    public float AngleCalculator(Transform sight, Transform standard)
    {
        Vector3 direction = standard.position - sight.position; //방향벡터 계산

        float angle = Vector3.Angle(sight.forward, direction);

        return angle;
    }

    public void LookPlayer_UI()
    {
        if (isLook)
        {
            Vector3 lookDirection = mainCamera.transform.position - canvas.transform.position;
            lookDirection.y = 0f;
            canvas.transform.rotation = Quaternion.LookRotation(-lookDirection);
        }
        else
        {
            //canvas.transform.rotation = Quaternion.Euler(0, 0, 0);
            return;
        }
    }

    public IEnumerator canvasFadeIn()
    {
        isOnUI = true;
        canGroup.alpha = 0;
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            float alpha = Mathf.Lerp(0, 1, elapsedTime);
            canGroup.alpha = alpha;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canGroup.alpha = 1;

        yield break;
    }


    public void Btn1(string contentText)
    {
        text.text = contentText;
    }

    public void ToggleLook()
    {
        if (isLook)
        {
            isLook = false;
            btnText.color = Color.red;
        }
        else
        {
            isLook = true;
            btnText.color = Color.black;
        }
    }
}
