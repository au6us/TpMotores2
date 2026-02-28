using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
    public int currentStamina; //El encargado de la que stamina baje

    [SerializeField] private int maxStamina = 10;
    [SerializeField] private float timeToChargeStamina = 10f;

    [SerializeField] TextMeshProUGUI staminaText = null;
    [SerializeField] TextMeshProUGUI timerText = null;

    DateTime nextStaminaTime; 
    DateTime lastStaminaTime;

    public bool recharging;


    private void Start()
    {
        LoadStamina();
        UpdateStaminaUI();
        UpdateTimerUI();
        StartCoroutine(AutoRechargeStamina());
    }

    IEnumerator AutoRechargeStamina()
    {
        recharging = true;

        while (currentStamina < maxStamina)
        {
            DateTime currentTime = DateTime.Now;
            DateTime nextTime = nextStaminaTime;

            bool staminaAdded = false;

            while(currentTime > nextTime)
            {
                if (currentStamina >= maxStamina) break;    

                currentStamina += 1;

                staminaAdded |= true;

                if(lastStaminaTime > nextTime)
                {
                    nextTime = lastStaminaTime;
                }

                nextTime = nextTime.AddSeconds(timeToChargeStamina);
            }

            if(staminaAdded)
            {
                nextStaminaTime = nextTime;
                lastStaminaTime = DateTime.Now;
                UpdateStaminaUI();
            }

            UpdateTimerUI();
            SaveStamina();

            yield return new WaitForEndOfFrame();

        }

        recharging = false;

    }

    public void UseStamina(int stamina)
    {
        if(currentStamina < stamina)
        return;

        currentStamina -= stamina;

        if(!recharging && currentStamina < maxStamina)
        {
            nextStaminaTime = DateTime.Now.AddSeconds(timeToChargeStamina);
            StartCoroutine(AutoRechargeStamina());
        }
        UpdateStaminaUI();
        SaveStamina();
    }

    public void RechargeStamina(int stamina)
    {
        currentStamina += stamina;

        if(recharging && currentStamina >= maxStamina)
        {
            StopAllCoroutines();
            recharging = false;
            UpdateTimerUI();
        }
        UpdateStaminaUI();
        SaveStamina();
    }

    public void UpdateStaminaUI()
    {
        staminaText.text = currentStamina.ToString() + "/" + maxStamina.ToString();
    }
    public void UpdateTimerUI()
    {
        if(currentStamina >= maxStamina)
        {
            timerText.text = "Full";

            return;
        }

        TimeSpan timer = nextStaminaTime - DateTime.Now;

        timerText.text = timer.Minutes.ToString("00") + ":" + timer.Seconds.ToString("00");
    }
    public void SaveStamina()
    {
        PlayerPrefs.SetInt("CurrentStamina", currentStamina);
        PlayerPrefs.SetString("NextStaminaTime", nextStaminaTime.ToString());
        PlayerPrefs.SetString("LastStaminaTime", lastStaminaTime.ToString());
        PlayerPrefs.Save();
    }
    public void LoadStamina()
    {
        currentStamina = PlayerPrefs.GetInt("CurrentStamina", maxStamina);
        nextStaminaTime = StringToDateTime(PlayerPrefs.GetString("NextStaminaTime"));
        lastStaminaTime = StringToDateTime(PlayerPrefs.GetString("LastStaminaTime"));
    }

    DateTime StringToDateTime(string date)
    {
        if (string.IsNullOrEmpty(date))
        {
            return DateTime.Now;
        }
        else
        {
            return DateTime.Parse(date);
        }
    }
}
