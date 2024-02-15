using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAction : MonoBehaviour
{
    private TextMeshProUGUI myText;
    public GameObject instantiateObj;

    private void Awake()
    {
        myText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void NextStep()
    {
        RecipeManager.instance.uiAction = this;
        RecipeManager.instance.ProcessChange(RecipeProcess.DetailRecipe, myText.text);
    }





}
