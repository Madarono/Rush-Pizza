using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

[System.Serializable]
public class CheckForIngrediant
{
    public PizzaOptions ingrediant;
    public string englishName;
    public string deutschName;
}

public class AdvancedFeedback : MonoBehaviour
{
    [Header("Checking Topping Amount")]
    public Lines[] tooMuch;
    public Lines[] tooLittle;

    [Header("Checking Rating")]
    public Lines[] terribleQuality;
    
    [Header("Checking Amount of cuts")]
    public Lines[] tooLittleCuts;
    public Lines[] tooMuchCuts;

    [Header("CheckCook")]
    public Lines[] overcooked;
    public Lines[] undercooked;

    [Header("CheckPizza")]
    public Lines[] hatedToppings;
    public Lines[] foreignToppings;

    [Header("Ingrediant Code")]
    public Settings settings;
    public string ingrediantCode;
    public string ingrediantName;
    public CheckForIngrediant[] ingrediantChecks;

    [Header("End Product")]
    public string dialogText;

    void Start()
    {
        if(settings == null)
        {
            settings = GameObject.Find("UniversalScripts").GetComponent<Settings>();
        }
    }


    public void TurnIngrediantToName(PizzaOptions ingrediant)
    {
        for(int i = 0; i < ingrediantChecks.Length; i++)
        {
            if(ingrediant == ingrediantChecks[i].ingrediant)
            {
                if(settings.english)
                {
                    ingrediantName = ingrediantChecks[i].englishName;
                }
                else
                {
                    ingrediantName = ingrediantChecks[i].deutschName;
                }
                break;
            }
        }
    }

    //Ways to argue
    public void CheckDifference(float difference)
    {
        if(difference > 0)
        {
            int random = UnityEngine.Random.Range(0, tooMuch.Length);
            if(settings.english)
            {
                dialogText = DeciferString(tooMuch[random].englishVersion);
            }
            else
            {
                dialogText = DeciferString(tooMuch[random].deutschVersion);
            }
        }
        else
        {
            int random = UnityEngine.Random.Range(0, tooLittle.Length);
            if(settings.english)
            {
                dialogText = DeciferString(tooLittle[random].englishVersion);
            }
            else
            {
                dialogText = DeciferString(tooLittle[random].deutschVersion);
            }
        }
    }

    public void CheckCuts(int cuts, int minAmount, int maxAmount)
    {
        if(cuts < minAmount)
        {
            if(settings.english)
            {
                dialogText = tooLittleCuts[UnityEngine.Random.Range(0, tooLittleCuts.Length)].englishVersion;
            }
            else
            {
                dialogText = tooLittleCuts[UnityEngine.Random.Range(0, tooLittleCuts.Length)].deutschVersion;
            }
        }
        else if(cuts > maxAmount)
        {
            if(settings.english)
            {
                dialogText = tooMuchCuts[UnityEngine.Random.Range(0, tooMuchCuts.Length)].englishVersion;
            }
            else
            {
                dialogText = tooMuchCuts[UnityEngine.Random.Range(0, tooMuchCuts.Length)].deutschVersion;
            }
        }
    }

    public void Checkcooked(int cook)
    {
        if(settings.english)
        {
            if(cook != 0)
            {
                dialogText = overcooked[UnityEngine.Random.Range(0, overcooked.Length)].englishVersion;
            }
            else
            {
                dialogText = undercooked[UnityEngine.Random.Range(0, undercooked.Length)].englishVersion;
            }
        }
        else
        {
            if(cook != 0)
            {
                dialogText = overcooked[UnityEngine.Random.Range(0, overcooked.Length)].deutschVersion;
            }
            else
            {
                dialogText = undercooked[UnityEngine.Random.Range(0, undercooked.Length)].deutschVersion;
            }
        }
    }

    public void BadQuality()
    {
        if(settings.english)
        {
            dialogText = terribleQuality[UnityEngine.Random.Range(0, terribleQuality.Length)].englishVersion;
        }
        else
        {
            dialogText = terribleQuality[UnityEngine.Random.Range(0, terribleQuality.Length)].deutschVersion;
        }
    }

    public void HatedTopping()
    {
        if(settings.english)
        {
            dialogText = DeciferString(hatedToppings[UnityEngine.Random.Range(0, hatedToppings.Length)].englishVersion);
        }
        else
        {
            dialogText = DeciferString(hatedToppings[UnityEngine.Random.Range(0, hatedToppings.Length)].deutschVersion);
        }
    }

    public void TooMuchForeignToppings()
    {
        if(settings.english)
        {
            dialogText = DeciferString(foreignToppings[UnityEngine.Random.Range(0, foreignToppings.Length)].englishVersion);
        }
        else
        {
            dialogText = DeciferString(foreignToppings[UnityEngine.Random.Range(0, foreignToppings.Length)].deutschVersion);
        }
    }

    string DeciferString(string decifer)
    {
        string newIngrediantName = "";
        if(decifer.StartsWith(ingrediantCode))
        {
            newIngrediantName = char.ToUpper(ingrediantName[0]) + ingrediantName.Substring(1);
        }
        else
        {
            newIngrediantName = ingrediantName.ToLower();
        }

        string product = decifer.Replace(ingrediantCode, newIngrediantName);

        return product;
    }
}