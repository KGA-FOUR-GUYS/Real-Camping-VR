using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSky : MonoBehaviour
{
    public enum dayTime
    {
        Morning = 6,
        Noon = 8,
        Evening = 18,
        Night = 20,
    }
    //public Material dayMat;
    //public Material nightMat;
    //public GameObject dayLight;
    //public GameObject nightLight;

    //public Color dayFog;
    //public Color nightFog;
    //private void Update()
    //{
    //    RenderSettings.skybox.SetFloat("_Rotation", Time.time * 0.5f);
    //}
    //private void OnGUI()
    //{
    //    if(GUI.Button(new Rect(5,5,80,20),"Day"))
    //    {
    //        RenderSettings.skybox = dayMat;
    //        RenderSettings.fogColor = dayFog;
    //        dayLight.SetActive(true);
    //        nightLight.SetActive(false);
    //    }
    //    if (GUI.Button(new Rect(5, 35, 80, 20), "Night"))
    //    {
    //        RenderSettings.skybox = nightMat;
    //        RenderSettings.fogColor = nightFog;
    //        dayLight.SetActive(false);
    //        nightLight.SetActive(true);
    //    }
    //}
    [Header("Time")]
    public float TimeScale = 0;
    [SerializeField]private dayTime _dayTime;

    [Header("Texture")]
    [SerializeField] private Texture2D skyboxNight;
    [SerializeField] private Texture2D skyboxSunrise;
    [SerializeField] private Texture2D skyboxDay;
    [SerializeField] private Texture2D skyboxSunset;

    [Header("Gradient")]
    [SerializeField] private Gradient gradientNightToSunrise;
    [SerializeField] private Gradient gradientSunriseToDay;
    [SerializeField] private Gradient gradientDayToSunset;
    [SerializeField] private Gradient gradientSunsetToNight;

    [Header("Light")]
    [SerializeField] private Light globalLight;

    [Header("M/H/D")]
    [SerializeField]  private int minutes;
    public int Minutes
    { get { return minutes; } set { minutes = value; OnMinutesChange(value); } }
    [SerializeField] private int hours;
    public int Hours
    { get { return hours; } set { hours = value; OnHoursChange(value); } }
    [SerializeField] private int days;
    public int Days
    { get { return days; } set { days = value; } }

    [SerializeField]private float tempSecond;
    [Range(1f, 60f)] public float MinutePerSecond = 1f;
    
    private void Start()
    {
        Hours = (int)_dayTime;
        //Time.timeScale = 50f;
        //RenderSettings.skybox.SetTexture("_Texture1", skyboxNight);
        //RenderSettings.skybox.SetTexture("_Texture2", skyboxNight);
        //RenderSettings.skybox.SetFloat("_Blend", 0);
        //globalLight.color = gradientNightToSunrise.Evaluate(0);
        //RenderSettings.fogColor = globalLight.color;
        //RenderSettings.skybox.SetFloat("_Exposure2", 0);
        //RenderSettings.skybox.SetFloat("_Exposure1", 0);
    }
    private void Update()
    {
        tempSecond += Time.deltaTime;
        if(tempSecond >= MinutePerSecond)
        {
            Minutes += 1;
            tempSecond = 0;
        }
        RenderSettings.skybox.SetFloat("_Rotation1", Time.time * 0.3f);
        RenderSettings.skybox.SetFloat("_Rotation2", Time.time * 0.3f);
    }
    private void OnMinutesChange(int value)
    {
        globalLight.transform.Rotate(Vector3.up, (1f / 1440f) * 360, Space.World);
        if(value >= 60)
        {
            Hours++;
            minutes = 0;
        }
        if(Hours >= 24)
        {
            Hours = 0;
            Days++;
        }
    }
    private void OnHoursChange(int value)
    {
        if(value == 6)//¾ÆÄ§
        {
            StartCoroutine(LerpSkybox(skyboxNight, skyboxSunrise, 100f));
            StartCoroutine(LerpLight(gradientNightToSunrise,100f));
        }
        else if(value == 8)//³·
        {
            StartCoroutine(LerpSkybox(skyboxSunrise, skyboxDay, 100f));
            StartCoroutine(LerpLight(gradientSunriseToDay,100f));
        }
        else if(value == 18)//Àú³á
        {
            StartCoroutine(LerpSkybox(skyboxDay, skyboxSunset, 100f));
            StartCoroutine(LerpLight(gradientDayToSunset, 100f));
        }
        else if(value == 20)//¹ã
        {
            StartCoroutine(LerpSkybox(skyboxSunset, skyboxNight, 100f));
            StartCoroutine(LerpLight(gradientSunsetToNight, 100f));
        }
    }
    private IEnumerator LerpSkybox(Texture2D a, Texture2D b, float time)
    {
        RenderSettings.skybox.SetTexture("_Texture1", a);
        RenderSettings.skybox.SetTexture("_Texture2", b);
        RenderSettings.skybox.SetFloat("_Blend", 0);
        for(float i = 0; i < time; i += Time.deltaTime)
        {
            RenderSettings.skybox.SetFloat("_Blend", i / time);
            yield return null;
        }
        RenderSettings.skybox.SetTexture("Texture1", b);
    }
    private IEnumerator LerpLight(Gradient lightGradient, float time)
    {
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            globalLight.color = lightGradient.Evaluate(i / time);
            RenderSettings.fogColor = globalLight.color;
            yield return null;
        }
    }
    //private IEnumerator LerpExposure(float time)
    //{
    //    for(float i = 0; i < time; i += Time.deltaTime)
    //    {
    //        RenderSettings.skybox.SetFloat("Exposure1", i / time);
    //    }
    //}
}
