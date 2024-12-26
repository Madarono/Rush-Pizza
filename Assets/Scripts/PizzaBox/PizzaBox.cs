using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PizzaRating
{
    Perfect,
    Great,
    Good,
    Bad,
    Terrible
}

[System.Serializable]
public class BoxInformation
{
    // public string ingrediantName;
    public PizzaOptions ingrediant;
    public int toppingCount;
    public int leftSideCount;
    public int rightSideCount;
    // public float ingrediantPercentage;
    // public float leftPercentage;
    // public float rightPercentage;

    public float averageDistance;
    public PizzaRating toppingDistanceRating;
}

[System.Serializable]
public class CutInformation
{
    public float numberOfCuts;
    public float numberOfSlices;

    public float averageDistance;
    public PizzaRating cutDistanceRating;
}

[System.Serializable]
public class PizzaRatingRequirement
{
    public PizzaRating rating;
    public float maxRequirement;
    public float minRequirement;
}

public class PizzaBox : MonoBehaviour
{
    [HideInInspector]public Ingrediants[] ingrediants;
    [HideInInspector]public PizzaCuts cuts;

    public BoxInformation[] toppingInfo;
    public CutInformation cutInfo;
    public bool isCooked;
    public int cookedTimes;

    public PizzaRating overallPizzaRating;

    [Header("Pizza Rating Requirements")]
    public PizzaRatingRequirement[] ratingReq;
    public PizzaRatingRequirement[] overallRatingReq;

    public void GetPizzaInformation(GameObject pizza)
    {
        Pizza pizzaScript = pizza.GetComponent<Pizza>();
        this.ingrediants = pizzaScript.ingrediants;
        this.cuts = pizzaScript.cuts;
        this.isCooked = pizzaScript.isCooked;
        this.cookedTimes = pizzaScript.cookedTimes;

        ExportDataIntoInformation();
        GivePizzaRating();
        GetOverrallRating();
    }

    public void ExportDataIntoInformation()
    {
        for(int i = 0; i < ingrediants.Length; i++)
        {
            // toppingInfo[i].ingrediantName = ingrediants[i].ingrediantName;
            toppingInfo[i].ingrediant = ingrediants[i].ingrediant;
            toppingInfo[i].leftSideCount = ingrediants[i].leftSideObj.Count;
            toppingInfo[i].rightSideCount = ingrediants[i].rightSideObj.Count;
            toppingInfo[i].averageDistance = ingrediants[i].averageDistance;
            toppingInfo[i].toppingCount = ingrediants[i].ingrediantObj.Count;
        }

        cutInfo.numberOfCuts = cuts.numberOfCuts;
        cutInfo.numberOfSlices = cuts.numberOfSlices;
        cutInfo.averageDistance = cuts.averageDistance;
    }

    public void GivePizzaRating()
    {
        for(int i = 0; i < toppingInfo.Length; i++)
        {
            for(int o = 0; o < ratingReq.Length; o++)
            {
                if(toppingInfo[i].averageDistance > ratingReq[o].maxRequirement && toppingInfo[i].averageDistance < ratingReq[o].minRequirement)
                {
                    toppingInfo[i].toppingDistanceRating = ratingReq[o].rating;
                }
            }
        }
    }

    public void GetOverrallRating()
    {
        float totalElements = 0f;
        float totalRating = 0f;

        for(int i = 0; i < toppingInfo.Length; i++)
        {
            if(toppingInfo[i].averageDistance <= 0)
            {
                continue;
            }

            switch(toppingInfo[i].toppingDistanceRating)
            {
                case PizzaRating.Terrible:
                    totalRating += 1f;
                    totalElements++;
                    break;

                case PizzaRating.Good:
                    totalRating += 2f;
                    totalElements++;
                    break;

                case PizzaRating.Great:
                    totalRating += 3f;
                    totalElements++;
                    break;

                case PizzaRating.Perfect:
                    totalRating += 4f;
                    totalElements++;
                    break;
            }
        }
        
        float overallRating = 0;
        overallRating = totalRating / totalElements;
        bool hasRating = false;
        for(int i = 0; i < overallRatingReq.Length; i++)
        {
            if(overallRating >= overallRatingReq[i].minRequirement && overallRating <= overallRatingReq[i].maxRequirement)
            {
                hasRating = true;
                overallPizzaRating = overallRatingReq[i].rating;
                Debug.Log("Finalized Rating");
                break;
            }
        }

        if(!hasRating)
        {
            overallPizzaRating = PizzaRating.Good;
        }
        Debug.Log(overallRating);
    }
}
