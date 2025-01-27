using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeChanges : MonoBehaviour
{
    public Material skyMaterial;

    public Color[] topGradientTransitions;
    public Color[] bottomGradientTransitions;

    private float t = 0f;
    private int currentIndex = 0;
    private float speedOfTransition;

    [Header("Timer")]
    public Settings settings;
    public bool h24Format;

    public float dayTimeInHours = 12;
    public float ingameHoursInSeconds = 12*60*60;

    public int startingHour = 11;
    public TextMeshProUGUI timeVisual;
    public Image sunVisual;

    private float timeMultiplyer;
    private float currentTime;

    void Start()
    {
        ResetTime();
        skyMaterial.SetColor("_SkyGradientTop", topGradientTransitions[0]);
        skyMaterial.SetColor("_SkyGradientBottom", bottomGradientTransitions[0]);
        speedOfTransition = 1f / ((ingameHoursInSeconds * 2) / topGradientTransitions.Length);
    }

    void Update()
    {
        h24Format = settings.h24Format;

        //Time
        if(currentTime <= (dayTimeInHours * 60 * 60))
        {
            currentTime += Time.deltaTime * timeMultiplyer;
            UpdateTime();
        }

        //Stop if current time is equal to dayTimeInHours
        if(currentTime >= (dayTimeInHours * 60 * 60))
        {
            return;
        }

        //Skybox Changes
        t += Time.deltaTime * speedOfTransition;

        Color newTopColor = Color.Lerp(topGradientTransitions[currentIndex], topGradientTransitions[(currentIndex + 1) % topGradientTransitions.Length], t);

        Color newBottomColor = Color.Lerp(bottomGradientTransitions[currentIndex], bottomGradientTransitions[(currentIndex + 1) % bottomGradientTransitions.Length], t);

        skyMaterial.SetColor("_SkyGradientTop", newTopColor);
        skyMaterial.SetColor("_SkyGradientBottom", newBottomColor);

        if (t >= 1f)
        {
            t = 0f;
            currentIndex = (currentIndex + 1) % topGradientTransitions.Length;
        }
    }

    public void ResetTime()
    {
        timeMultiplyer =  (dayTimeInHours * 60 * 60) / ingameHoursInSeconds;
        Debug.Log(timeMultiplyer);
        currentTime = 0;
    }

    void UpdateTime()
    {
        //CurrentTime = 60
        sunVisual.fillAmount = currentTime / (dayTimeInHours * 60 * 60);

        int hours = Mathf.FloorToInt((int)currentTime / 3600) % 24;
        int minutes = Mathf.FloorToInt(((int)currentTime % 3600) / 60);
        
        if(minutes % 10 == 0)
        {
            // Debug.Log(hours + ":" + minutes);   
            
            int hourCombined = startingHour + hours;
            if(h24Format)
            {
                if(minutes < 10)
                {
                    timeVisual.text = hourCombined + ":0" + minutes;
                }
                else
                {
                    timeVisual.text = hourCombined + ":" + minutes;
                }
            }
            else
            {
                if(hourCombined > 12)
                {
                    int afterRemoving12 = hourCombined - 12;
                    if(settings.english)
                    {
                        if(minutes < 10)
                        {
                            timeVisual.text = afterRemoving12 + ":0" + minutes + " PM";
                        }
                        else
                        {
                            timeVisual.text = afterRemoving12 + ":" + minutes + " PM";
                        }
                    }
                    else
                    {
                        if(minutes < 10)
                        {
                            timeVisual.text = afterRemoving12 + ":0" + minutes + " Abends";
                        }
                        else
                        {
                            timeVisual.text = afterRemoving12 + ":" + minutes + " Abends";
                        }
                    }
                }
                else
                {
                    if(settings.english)
                    {
                        if(minutes < 10)
                        {
                            timeVisual.text = hourCombined + ":0" + minutes + " AM";
                        }
                        else
                        {
                            timeVisual.text = hourCombined + ":" + minutes + " AM";
                        }
                    }
                    else
                    {
                        if(minutes < 10)
                        {
                            timeVisual.text = hourCombined + ":0" + minutes + " Morgen";
                        }
                        else
                        {
                            timeVisual.text = hourCombined + ":" + minutes + " Morgen";
                        }
                    }
                }
            }
        }

    }
}
