using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TipAdditions
{
    public PizzaRating rating;
    public float percentageOfTip;
}

[System.Serializable]
public class PatienceTip
{
    public float minPatience;
    public float percentageOfTip;
}

public class AdvancedTippingSystem : MonoBehaviour
{
    public float finalTip;
    public float tip;

    public float perfectCookingTip = 2f;
    public float perfectCutTip = 2f;
    public PatienceTip[] patienceTip;

    public TipAdditions[] toppingTip;

    public void AddToTip(PizzaRating rating, float price)
    {
        for(int i = 0; i < toppingTip.Length; i++)
        {
            if(rating == toppingTip[i].rating)
            {
                tip += price / toppingTip[i].percentageOfTip;
                break;
            }
        }
    }

    public void OtherTip(int current, int min, int max, float perfectTip) //3, 1, 3
    {
        if(max > 4)
        {
            max = 4;
        }

        if(current > max || current == 4)
        {
            tip += 0;
            return;
        }

        tip += perfectTip * (min / current);
    }

    public void CalculateFinalTip(float patience, float totalPatience)
    {
        float percentage = (patience / totalPatience) * 100f;
        finalTip = tip;


        for(int i = 0; i < patienceTip.Length; i++)
        {
            if(percentage <= patienceTip[i].minPatience)
            {
                finalTip += (finalTip * patienceTip[i].percentageOfTip);
                break;
            }
        }

        Debug.Log("Normal Tip: " + tip.ToString() + ", FinalTip: " + finalTip.ToString());
    }
}
