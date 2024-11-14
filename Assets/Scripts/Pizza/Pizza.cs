using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pizza : MonoBehaviour
{
    public string redSauceName = "Tomato Sauce";
    public List<GameObject> redSauce;
    public float saucePercentage;
    public float maxSaucePercetage = 50f;
    
    public string cheeseName = "Cheese";
    public List<GameObject> cheese;
    public float cheesePercentage;
    public float maxCheesePercentage = 50f;

    public void CheckName(string name, GameObject obj)
    {
        switch(name)
        {
            case var _ when name == redSauceName:
                redSauce.Add(obj);
                break;

            case var _ when name == cheeseName:
                cheese.Add(obj);
                break;

            default:
                Debug.LogWarning($"Unknown topping or ingredient: {name}");
                break;
        }
        UpdatePercentage();
    }

    public void UpdatePercentage()
    {
        saucePercentage = redSauce.Count / maxSaucePercetage;
        if(saucePercentage > 1)
        {
            saucePercentage = 1f;
        }

        cheesePercentage = cheese.Count / maxCheesePercentage;
        if(cheesePercentage > 1)
        {
            cheesePercentage = 1f;
        }
    }
}
