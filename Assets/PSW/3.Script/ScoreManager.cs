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
    [SerializeField] ObjectSpawner[] Ingredient_Spawner_Arr;
    public List<string> Ingredients_Name = new List<string>(); //레시피 재료의 이름들

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

    private void Start()
    {
        //재료 스포너 찾아서 배열로!
        //Ingredient_Spawner_Arr = FindObjectsOfType<ObjectSpawner>();
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
        StartCoroutine(Find_Recipe_Ingredients_Co(select_Recipe));
    }

    private IEnumerator Find_Recipe_Ingredients_Co(RecipeSO select_Recipe)
    {
        currentRecipe = select_Recipe;
        yield return null;

        Ingredient_Spawner_Arr = FindObjectsOfType<ObjectSpawner>();
        yield break;
    }

    public void Cut_Judge()
    {
        //i번째 재료의 자식들(자른거 또는 통짜)의 부피가 타겟 볼륨인지 확인하여 점수판정
        for (int i = 0; i < Ingredient_Spawner_Arr.Length; i++)
        {
            int Correct_Volume = 0;
            MeshCalculator[] ingredients_volume = Ingredient_Spawner_Arr[i].objectPool.GetComponentsInChildren<MeshCalculator>(false);
            int List_Num = (int)Matching_Name(Ingredient_Spawner_Arr[i]);
            var Target_Volume = currentRecipe.ingredientList[List_Num].sliceVolume;
            for (int j = 0; j < ingredients_volume.Length; j++)
            {
                if (ingredients_volume[j].Volume <= Target_Volume)
                {
                    Correct_Volume++;
                }
            }
            int piece_count = Active_ChildCount(Ingredient_Spawner_Arr[i].objectPool);
            float i_cut_score = Mathf.Floor(Correct_Volume * 100 / piece_count);

            if (piece_count == currentRecipe.ingredientList[List_Num].sliceCount * currentRecipe.ingredientList[List_Num].quantity)
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
        Total_Cut_Score = Mathf.Floor(Total_Cut_Score / Ingredient_Spawner_Arr.Length);
    }

    private int? Matching_Name(ObjectSpawner spawner)
    {
        int? Name_Num = null;
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
            }
        }
        return Name_Num;
    }

    private int Active_ChildCount(Transform objectPool)
    {
        int activeChildCount = 0;
        for (int i = 0; i < objectPool.childCount; i++)
        {
            if (objectPool.GetChild(i).gameObject.activeSelf)
            {
                activeChildCount++;
            }
        }
        return activeChildCount;
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
            int List_Num = (int)Matching_Name(Ingredient_Spawner_Arr[i]);
            var Target_Ripe = currentRecipe.ingredientList[List_Num].ripeState;
            var Target_Boil = currentRecipe.ingredientList[List_Num].ripeByBoil;
            var Target_Broil = currentRecipe.ingredientList[List_Num].ripeByBroil;
            var Target_Grill = currentRecipe.ingredientList[List_Num].ripeByGrill;

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
            int piece_count = Active_ChildCount(Ingredient_Spawner_Arr[i].objectPool);
            piece_count -= Except_Child;
            float i_Ripe_score = Mathf.Floor(Ripe_sum * 20f / piece_count);
            float i_Boil_score = Mathf.Round(Boil_sum * 100 / piece_count);
            float i_Broil_score = Mathf.Round(Broil_sum * 100 / piece_count);
            float i_Grill_score = Mathf.Round(Grill_sum * 100 / piece_count);

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
        if (currentRecipe == null)
        {
            Debug.Log("선택된 레시피가 없으요");
            return;
        }

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
