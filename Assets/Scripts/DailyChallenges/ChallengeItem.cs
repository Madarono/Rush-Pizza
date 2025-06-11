using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public class ChallengeItem : MonoBehaviour
{
    public Settings settings;
    public Stats stats;
    public Challenge challenge;
    public DailyChallenges manager;
    public TextMeshProUGUI visual;
    public Color[] visualColor; //0 for false, 1 for true

    private float reward;
    private float initialValue;
    private float value;

    void Start()
    {
        Refresh();
    }

    void RefreshValues()
    {
        reward = challenge.reward;
        initialValue = (float)challenge.valueNeeded;
        value = manager.store[challenge.indexOfValue].value;
    }

    public void Refresh()
    {
        RefreshValues();

        if(settings.english)
        {
            if(manager.store[challenge.indexOfValue].value % 1 != 0)
            {
                visual.text = "- " + challenge.englishVisual + " (" + manager.store[challenge.indexOfValue].value.ToString("F2") + ") - " + challenge.reward.ToString() + "$";
            }
            else
            {
                visual.text = "- " + challenge.englishVisual + " (" + manager.store[challenge.indexOfValue].value.ToString("F0") + ") - " + challenge.reward.ToString() + "$";
            }
        }
        else
        {
            if(manager.store[challenge.indexOfValue].value % 1 != 0)
            {
                visual.text = "- " + challenge.deutschVisual + " (" + manager.store[challenge.indexOfValue].value.ToString("F2", new CultureInfo("de-DE")) + ") - " + challenge.reward.ToString() + "€";
            }
            else
            {
                visual.text = "- " + challenge.deutschVisual + " (" + manager.store[challenge.indexOfValue].value.ToString("F0") + ") - " + challenge.reward.ToString() + "€";
            }
        }

        if(challenge.lessThan)
        {
            if(value <= initialValue)
            {
                visual.color = visualColor[1];
            }
            else
            {
                visual.color = visualColor[0];
            }
        }
        else
        {
            if(value >= initialValue)
            {
                visual.color = visualColor[1];
            }
            else
            {
                visual.color = visualColor[0];
            }
        }
    }

    public void CheckConditions()
    {
        RefreshValues();

        if(challenge.lessThan)
        {
            if(value <= initialValue)
            {
                settings.money += reward;
                stats.dailyChallengeProfit += reward;
            }
        }
        else
        {
            if(value >= initialValue)
            {
                settings.money += reward;
                stats.dailyChallengeProfit += reward;
            }
        }
    }
}