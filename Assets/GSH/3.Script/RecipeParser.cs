using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeParser : MonoBehaviour
{
    public Recipe[] Parse(string _CSVFileName)
    {
        List<Recipe> recipeList = new List<Recipe>();//레시피 리스트 생성
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName);//csv파일 가져옴
        string[] data = csvData.text.Split(new char[]{'\n'});

        for(int i = 1; i< data.Length;)
        {
            string[] row = data[i].Split(new char[] { ',' });

            Recipe recipe = new Recipe();
            recipe.id = int.Parse(row[0]);
            recipe.name = row[1];
            List<string> progressList = new List<string>();

            do
            {
                progressList.Add(row[2]);
                if (++i < data.Length)
                {
                    row = data[i].Split(new char[] { ',' });
                }
                else
                {
                    break;
                }
            }
            while (row[0].ToString() == "");

            recipe.progress = progressList.ToArray();

            recipeList.Add(recipe);
        }
        return recipeList.ToArray();
    }
}
