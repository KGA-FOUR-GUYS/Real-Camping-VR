using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModalController : MonoBehaviour
{
    public static ModalController instance = null;
    private Canvas modalCanvas;


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

        TryGetComponent(out modalCanvas);
        modalCanvas.enabled = true;
        Time.timeScale = 0f;
    }


    public void CloseModalBtn()
    {
        modalCanvas.enabled = false;
        Time.timeScale = 1f;

    }

}
