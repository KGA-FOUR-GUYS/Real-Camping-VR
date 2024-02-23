using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Recipe_UI : MonoBehaviour
{
    public Image cookingImg;
    public TextMeshProUGUI myText;
    public Image[] stars;
    public GameObject instantiateObj;
    public RecipeSO currentRecipeSO;


    private void Awake()
    {
        myText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void NextStep()
    {
        RecipeManager.instance.recipe_UI = this;
        RecipeManager.instance.currentSO = currentRecipeSO;
        RecipeManager.instance.ProcessChange(RecipeProcess.DetailRecipe, myText.text);
        SoundManager.instance.PlayCookingSFX(0);
    }





}
