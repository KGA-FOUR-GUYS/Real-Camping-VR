using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Temp_ScoreUI : MonoBehaviour
{
    public static Temp_ScoreUI instance = null;
    public TextMeshProUGUI text;

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
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateScore()
    {
        ScoreManager.Instance.Cut_Judge();
        ScoreManager.Instance.Ripe_Judge();
        ScoreManager.Instance.Total_Score_Judge();
        text.text = $"자르기점수 : {ScoreManager.Instance.Total_Cut_Score} \n" +
                    $"굽기점수 : {ScoreManager.Instance.Total_Ripe_Score}\n" +
                    $"총합점수 : {ScoreManager.Instance.Total_Score}\n";
    }
}
