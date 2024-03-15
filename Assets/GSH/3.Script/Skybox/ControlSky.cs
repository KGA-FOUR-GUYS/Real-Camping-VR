using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ControlSky : NetworkBehaviour
{
    public enum dayTime
    {
        Morning = 6,
        Noon = 8,
        Evening = 18,
        Night = 20,
    }
    
    [Header("Time")]
    public float TimeScale = 0;
    [SerializeField] private dayTime _dayTime;

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
    [SyncVar(hook = nameof(SyncMinute))]
    [SerializeField] private int minutes;
    public int Minutes
    {
        get
        {
            return minutes;
        }
        set
        {
            minutes = value;
            OnMinutesChange(value);
        }
    }
    private void SyncMinute(int _, int newValue)
    {
        minutes = newValue;
    }

    [SyncVar(hook = nameof(SyncHour))]
    [SerializeField] private int hours;
    public int Hours
    {
        get
        {
            return hours;
        }
        set
        {
            hours = value;
            OnHoursChange(value);
        }
    }
    private void SyncHour(int _, int newValue)
    {
        hours = newValue;
    }

    [SyncVar(hook = nameof(SyncDay))]
    [SerializeField] private int days;
    public int Days
    {
        get
        {
            return days;
        }
        set
        {
            days = value;
        }
    }
    private void SyncDay(int _, int newValue)
    {
        days = newValue;
    }

    [SyncVar(hook = nameof(SyncTempSecond))]
    [SerializeField]private float tempSecond;
    private void SyncTempSecond(float _, float newValue)
    {
        tempSecond = newValue;
    }

    [Range(1f, 60f)] public float MinutePerSecond = 1f;

    private void Start()
    {
        if (!isServer) return;

        Hours = (int)_dayTime;
    }
    private void Update()
    {
        if (!isServer) return;

        tempSecond += Time.deltaTime;
        if(tempSecond >= MinutePerSecond)
        {
            Minutes += 1;
            tempSecond = 0;
        }

        RenderSettings.skybox.SetFloat("_Rotation1", Time.time * .3f);
        RenderSettings.skybox.SetFloat("_Rotation2", Time.time * .3f);
        RpcSetSkyboxRotation(Time.time * .3f);
    }
    [ClientRpc]
    private void RpcSetSkyboxRotation(float value)
    {
        RenderSettings.skybox.SetFloat("_Rotation1", value);
        RenderSettings.skybox.SetFloat("_Rotation2", value);
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
        RpcSetTexture1(a);
        RenderSettings.skybox.SetTexture("_Texture2", b);
        RpcSetTexture2(b);

        RenderSettings.skybox.SetFloat("_Blend", 0);
        RpcSetBlend(0);
        for(float i = 0; i < time; i += Time.deltaTime)
        {
            RenderSettings.skybox.SetFloat("_Blend", i / time);
            RpcSetBlend(i / time);
            yield return null;
        }

        RenderSettings.skybox.SetTexture("_Texture1", b);
        RpcSetTexture1(b);
    }
    [ClientRpc]
    private void RpcSetTexture1(Texture2D texture)
    {
        RenderSettings.skybox.SetTexture("_Texture1", texture);
    }
    [ClientRpc]
    private void RpcSetTexture2(Texture2D texture)
    {
        RenderSettings.skybox.SetTexture("_Texture2", texture);
    }
    [ClientRpc]
    private void RpcSetBlend(float value)
    {
        RenderSettings.skybox.SetFloat("_Blend", value);
    }

    private IEnumerator LerpLight(Gradient lightGradient, float time)
    {
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            var newColor = lightGradient.Evaluate(i / time);
            globalLight.color = newColor;
            RpcSetLightColor(newColor);
            yield return null;
        }
    }
    [ClientRpc]
    private void RpcSetLightColor(Color newColor)
    {
        globalLight.color = newColor;
    }
}
