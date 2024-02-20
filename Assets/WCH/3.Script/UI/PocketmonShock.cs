using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PocketmonShock : MonoBehaviour
{
    public Image image;
    public bool isGo;
    Color color = new Color(170 / 255f, 76 / 255f, 250 / 255f, 1);
    private void Start()
    {
        if(isGo)
        StartCoroutine(LetsDoThis());
    }
    private IEnumerator LetsDoThis()
    {
        while (true)
        {
            image.color = Color.red;
            yield return new WaitForSeconds(0.02f);
            image.color = color;
            yield return new WaitForSeconds(0.02f);
            yield return null;
        }
    }
}
