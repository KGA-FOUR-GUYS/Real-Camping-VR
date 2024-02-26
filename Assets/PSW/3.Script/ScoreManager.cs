using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Cooking;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] RecipeSO currentRecipe;
    [SerializeField] int Error_Range = 5;
    [SerializeField] ObjectSpawner[] Ingredient_Spawner_Arr;
    [SerializeField] List<string> Ingredients_Name = new List<string>();
    [SerializeField] List<float> Cut_Scores = new List<float>();
    [SerializeField] List<bool> Cut_pieces = new List<bool>();
    [SerializeField] float Total_Cut_Score;
    [SerializeField] List<float> Ripe_Scores = new List<float>();
    [SerializeField] List<float> Ripe_Boil = new List<float>();//���
    [SerializeField] List<float> Ripe_Broil = new List<float>();//Ƣ���
    [SerializeField] List<float> Ripe_Grill = new List<float>();//����
    [SerializeField] float Total_Ripe_Score;
    [SerializeField] float Total_Score = 0f;
    int? Name_Num = null;

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

    private void Start()
    {
        //레시피 받아오는건 나중에 수정해줘
        currentRecipe = Resources.Load<RecipeSO>("RecipeSO/BeefStew");

        //재료 스포너 찾아서 배열로!
        Ingredient_Spawner_Arr = FindObjectsOfType<ObjectSpawner>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Total_Score_Judge();
        }
    }


    public void Cut_Judge()
    {
        //i번째 재료의 자식들(자른거 또는 통짜)의 부피가 타겟 볼륨인지 확인하여 점수판정
        for (int i = 0; i < Ingredient_Spawner_Arr.Length; i++)
        {
            int Correct_Volume = 0;
            MeshCalculator[] ingredients_volume = Ingredient_Spawner_Arr[i].objectPool.GetComponentsInChildren<MeshCalculator>(false);
            var Target_Volume = currentRecipe.ingredientList[(int)Matching_Name(Ingredient_Spawner_Arr[i])].sliceVolume;
            for (int j = 0; j < ingredients_volume.Length; j++)
            {
                if (ingredients_volume[j].Volume <= Target_Volume)
                {
                    Correct_Volume++;
                }
            }
            float i_cut_score = Mathf.Floor(Correct_Volume / Ingredient_Spawner_Arr[i].objectPool.childCount * 100);

            if (Ingredient_Spawner_Arr[i].objectPool.childCount == currentRecipe.ingredientList[(int)Matching_Name(Ingredient_Spawner_Arr[i])].sliceCount)
            {
                Cut_pieces.Add(true);
            }
            else
            {
                Cut_pieces.Add(false);
                i_cut_score -= 5f;
            }
            Cut_Scores.Add(i_cut_score);
        }
        foreach (var scores in Cut_Scores)
        {
            Total_Cut_Score += scores;
        }
        Total_Cut_Score = Mathf.Floor(Total_Cut_Score / Ingredient_Spawner_Arr.Length);
        if (Total_Cut_Score < 0)
        {
            Total_Cut_Score = 0;
        }
    }

    private int? Matching_Name(ObjectSpawner spawner)
    {
        for (int i = 0; i < currentRecipe.ingredientList.Count; i++)
        {
            var Ingredient_name = spawner.prefab.GetComponent<IngredientDataManager>().data.name;
            if (Ingredient_name.Equals($"{currentRecipe.ingredientList[i].name}"))
            {
                Name_Num = i;
                break;
            }
            else
            {
                Name_Num = null;
                Debug.Log($"{Ingredient_name}가(이) 레시피에 없습니다");
            }
        }
        return Name_Num;
    }

    private void Save_Ingredients_Name()
    {
        for (int i = 0; i < Ingredient_Spawner_Arr.Length; i++)
        {
            Ingredients_Name.Add(currentRecipe.ingredientList[(int)Matching_Name(Ingredient_Spawner_Arr[i])].name);
        }
    }

    public void Ripe_Judge()
    {
        for (int i = 0; i < Ingredient_Spawner_Arr.Length; i++)
        {
            float Ripe_sum = 0;
            float Boil_sum = 0;
            float Broil_sum = 0;
            float Grill_sum = 0;
            int Except_Child = 0;
            IngredientDataManager[] Ripe_Data = Ingredient_Spawner_Arr[i].objectPool.GetComponentsInChildren<IngredientDataManager>(false);
            var Target_Ripe = currentRecipe.ingredientList[(int)Matching_Name(Ingredient_Spawner_Arr[i])].ripeState;
            var Target_Boil = currentRecipe.ingredientList[(int)Matching_Name(Ingredient_Spawner_Arr[i])].ripeByBoil;
            var Target_Broil = currentRecipe.ingredientList[(int)Matching_Name(Ingredient_Spawner_Arr[i])].ripeByBroil;
            var Target_Grill = currentRecipe.ingredientList[(int)Matching_Name(Ingredient_Spawner_Arr[i])].ripeByGrill;
            for (int j = 0; j < Ripe_Data.Length; j++)
            {
                if (Ripe_Data[j].RipeState == RipeState.None || Ripe_Data[j].RipeState == RipeState.Raw)
                {
                    Ripe_Scores.Add(0f);
                    Except_Child++;
                    break;
                }
                switch (Mathf.Abs(Ripe_Data[j].RipeState - Target_Ripe))
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
                Boil_sum += Mathf.Round(Ripe_Data[j]._ripeByBoil / Ripe_Data[j].Ripe * 100f);
                Broil_sum += Mathf.Round(Ripe_Data[j]._ripeByBroil / Ripe_Data[j].Ripe * 100f);
                Grill_sum += Mathf.Round(Ripe_Data[j]._ripeByGrill / Ripe_Data[j].Ripe * 100f);
            }
            //총 점수 및 재료별 익힘 정도 계산
            float i_Ripe_score = Mathf.Floor(Ripe_sum * 20f / (Ingredient_Spawner_Arr[i].objectPool.childCount-Except_Child));
            float i_Boil_score = Mathf.Round(Boil_sum / (Ingredient_Spawner_Arr[i].objectPool.childCount - Except_Child) * 100);
            float i_Broil_score = Mathf.Round(Broil_sum / (Ingredient_Spawner_Arr[i].objectPool.childCount - Except_Child) * 100);
            float i_Grill_score = Mathf.Round(Grill_sum / (Ingredient_Spawner_Arr[i].objectPool.childCount - Except_Child) * 100);

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
        foreach (var scores in Ripe_Scores)
        {
            Total_Ripe_Score += scores;
        }
        Total_Ripe_Score = Mathf.Floor(Total_Ripe_Score / Ingredient_Spawner_Arr.Length);
    }

    public void Total_Score_Judge()
    {
        Save_Ingredients_Name();
        Cut_Judge();
        Ripe_Judge();
        Total_Score = Mathf.Round((Total_Cut_Score + Total_Ripe_Score) * 0.5f);

        //디버그용
        Debug.Log($"종합 점수 : {Total_Score}");
        Debug.Log($"자르기 점수 : {Total_Cut_Score}");
        for (int i = 0; i < Ingredients_Name.Count; i++)
        {
            Debug.Log($"자르기{Ingredients_Name[i]} : {Cut_Scores[i]}점 / 자르기 개수 : {Cut_pieces[i]}");
        }
        Debug.Log($"익히기 점수 : {Total_Ripe_Score}");
        for (int i = 0; i < Ingredients_Name.Count; i++)
        {
            Debug.Log($"익히기{Ingredients_Name[i]} : {Ripe_Scores[i]}점 / Boil : {Ripe_Boil[i]} / Broil : {Ripe_Broil[i]} / Grill : {Ripe_Grill[i]}");
        }

    }


}
