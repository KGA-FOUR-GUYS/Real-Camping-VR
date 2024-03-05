using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Cooking;

public class ScoreManager : MonoBehaviour
{
    [Header("레시피 정보")]
    [SerializeField] public RecipeSO currentRecipe;
    [Tooltip("익힘범위의 오차 ±")]
    [SerializeField] int Error_Range = 5;
    [SerializeField] List<IngredientDataManager> Ingredients_list = new List<IngredientDataManager>();

    [Header("자르기 판정")]
    public float Total_Cut_Score; //해당 레시피의 자르기 점수
    public List<float> Cut_Scores = new List<float>(); // 재료별 자르기 점수
    public List<bool> Cut_pieces = new List<bool>(); //재료별 조각개수

    [Header("익히기 판정")]
    public float Total_Ripe_Score; //해당 레시피의 익히기 점수
    public List<float> Ripe_Scores = new List<float>(); // 재료별 익히기 점수
    public List<float> Ripe_Boil = new List<float>(); // 재료별 끓이기 정도
    public List<float> Ripe_Broil = new List<float>(); // 재료별 볶기(후라이팬조리) 정도
    public List<float> Ripe_Grill = new List<float>(); // 재료별 굽기(직화구이) 정도

    [Header("총점")]
    public float Total_Score = 0f;

    //싱글톤
    public static ScoreManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Total_Score_Judge();
        }
    }

    public void Select_Recipe(RecipeSO select_Recipe)
    {
        currentRecipe = select_Recipe;
    }

    private void Find_Ingredients()
    {
        if (Ingredients_list != null)
        {
            Ingredients_list = null;
        }
        var _Ingredients = GetComponents<IngredientDataManager>();
        foreach (var _Ingredient in _Ingredients)
        {
            Ingredients_list.Add(_Ingredient);
        }
    }

    public void Cut_Judge()
    {
        Find_Ingredients();
        for (int i = 0; i < currentRecipe.ingredientList.Count; i++)
        {
            int Correct_Volume = 0;
            var Target_Volume = currentRecipe.ingredientList[i].sliceVolume;
            int slice_Num = 0;
            for (int j = 0; j < Ingredients_list.Count; j++)
            {
                if (currentRecipe.ingredientList[i].name == Ingredients_list[j].data.name)
                {
                    slice_Num++;
                    MeshCalculator ingredient_volume = Ingredients_list[j].transform.GetComponent<MeshCalculator>();
                    if (ingredient_volume.Volume <= Target_Volume)
                    {
                        Correct_Volume++;
                    }
                }
            }
            float i_cut_score = Mathf.Floor(Correct_Volume * 100 / slice_Num);
            if (slice_Num == currentRecipe.ingredientList[i].sliceCount * currentRecipe.ingredientList[i].quantity)
            {
                Cut_pieces.Add(true);
            }
            else
            {
                Cut_pieces.Add(false);
                i_cut_score -= 5f;
            }
            if (i_cut_score < 0)
            {
                i_cut_score = 0;
            }
            Cut_Scores.Add(i_cut_score);
        }
        foreach (var scores in Cut_Scores)
        {
            Total_Cut_Score += scores;
        }
        Total_Cut_Score = Mathf.Floor(Total_Cut_Score / Cut_Scores.Count);
    }

    public void Ripe_Judge()
    {
        if (Ingredients_list == null)
        {
            Find_Ingredients();
        }


        for (int i = 0; i < currentRecipe.ingredientList.Count; i++)
        {
            var Target_Ripe = currentRecipe.ingredientList[i].ripeState;
            var Target_Boil = currentRecipe.ingredientList[i].ripeByBoil;
            var Target_Broil = currentRecipe.ingredientList[i].ripeByBroil;
            var Target_Grill = currentRecipe.ingredientList[i].ripeByGrill;
            float Ripe_sum = 0;
            float Boil_sum = 0;
            float Broil_sum = 0;
            float Grill_sum = 0;
            int slice_Num = 0;
            //익힘목표가 없거나 생것이면 익히기 점수 제외해
            if (Target_Ripe != RipeState.None && Target_Ripe != RipeState.Raw)
            {
                for (int j = 0; j < Ingredients_list.Count; j++)
                {
                    if (currentRecipe.ingredientList[i].name == Ingredients_list[j].data.name)
                    {
                        slice_Num++;
                        switch (Mathf.Abs(Ingredients_list[j].RipeState - Target_Ripe))
                        {
                            case 0:
                                Ripe_sum += 5f;
                                break;
                            case 1:
                                Ripe_sum += 4f;
                                break;
                            case 2:
                                Ripe_sum += 3f;
                                break;
                            case 3:
                                Ripe_sum += 2f;
                                break;
                            case 4:
                                Ripe_sum += 1f;
                                break;
                        }
                        //익힘 비율 계산해서 총합에 넣어줌
                        Boil_sum += Mathf.Round(Ingredients_list[j]._ripeByBoil / Ingredients_list[j].Ripe * 100f);
                        Broil_sum += Mathf.Round(Ingredients_list[j]._ripeByBroil / Ingredients_list[j].Ripe * 100f);
                        Grill_sum += Mathf.Round(Ingredients_list[j]._ripeByGrill / Ingredients_list[j].Ripe * 100f);
                    }
                }
                float i_Ripe_score = Mathf.Floor(Ripe_sum * 20f / slice_Num);
                float i_Boil_score = Mathf.Round(Boil_sum * 100 / slice_Num);
                float i_Broil_score = Mathf.Round(Broil_sum * 100 / slice_Num);
                float i_Grill_score = Mathf.Round(Grill_sum * 100 / slice_Num);

                //익힘 비율 틀리면 감점
                if (i_Boil_score < (Target_Boil - Error_Range) || i_Boil_score > (Target_Boil + Error_Range))
                {
                    i_Ripe_score -= 5f;
                }
                if (i_Broil_score < (Target_Broil - Error_Range) || i_Broil_score > (Target_Broil + Error_Range))
                {
                    i_Ripe_score -= 5f;
                }
                if (i_Grill_score < (Target_Grill - Error_Range) || i_Grill_score > (Target_Grill + Error_Range))
                {
                    i_Ripe_score -= 5f;
                }
                if (i_Ripe_score < 0)
                {
                    i_Ripe_score = 0;
                }

                //점수 저장
                Ripe_Scores.Add(i_Ripe_score);
                Ripe_Boil.Add(i_Boil_score);
                Ripe_Broil.Add(i_Broil_score);
                Ripe_Grill.Add(i_Grill_score);
            }
        }
        foreach (var scores in Ripe_Scores)
        {
            Total_Ripe_Score += scores;
        }
        Total_Ripe_Score = Mathf.Floor(Total_Ripe_Score / Ripe_Scores.Count);
    }

    public void Total_Score_Judge()
    {
        if (currentRecipe == null)
        {
            Debug.Log("선택된 레시피가 없으요");
            return;
        }
        if (Cut_Scores == null)
        {
            Cut_Judge();
        }
        if (Ripe_Scores == null)
        {
            Ripe_Judge();
        }
        Total_Score = Mathf.Round((Total_Cut_Score + Total_Ripe_Score) * 0.5f);

        //디버그용
        Debug.Log($"종합 점수 : {Total_Score}");
        Debug.Log($"자르기 점수 : {Total_Cut_Score}");
        for (int i = 0; i < currentRecipe.ingredientList.Count; i++)
        {
            Debug.Log($"자르기{currentRecipe.ingredientList[i].name} : {Cut_Scores[i]}점 / 자르기 개수 : {Cut_pieces[i]}");
        }
        Debug.Log($"익히기 점수 : {Total_Ripe_Score}");
        for (int i = 0; i < currentRecipe.ingredientList.Count; i++)
        {
            if (currentRecipe.ingredientList[i].ripeState == RipeState.None
                || currentRecipe.ingredientList[i].ripeState == RipeState.Raw)
            {
                Debug.Log($"익히기{currentRecipe.ingredientList[i].name} : 익힘조리없음");
            }
            else
            {
                Debug.Log($"익히기{currentRecipe.ingredientList[i].name} : {Ripe_Scores[i]}점 / Boil : {Ripe_Boil[i]} / Broil : {Ripe_Broil[i]} / Grill : {Ripe_Grill[i]}");
            }
        }
    }


}
