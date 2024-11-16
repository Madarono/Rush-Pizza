using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ingrediants
{
    public string ingrediantName;
    public List<GameObject> ingrediantObj;
    public float ingrediantPercentage;
    public float maxIngrediantPercentage = 100f;
}

public class Pizza : MonoBehaviour
{
    public Ingrediants[] ingrediants;

    public void CheckName(string name, GameObject obj)
    {
        for(int i = 0; i < ingrediants.Length; i++)
        {
            if(name == ingrediants[i].ingrediantName)
            {
                ingrediants[i].ingrediantObj.Add(obj);
                break;
            }
        }
        UpdatePercentage();
    }

    public void UpdatePercentage()
    {
        for(int i = 0; i < ingrediants.Length; i++)
        {
            ingrediants[i].ingrediantPercentage = ingrediants[i].ingrediantObj.Count / ingrediants[i].maxIngrediantPercentage;
            if(ingrediants[i].ingrediantPercentage > 1f)
            {
                ingrediants[i].ingrediantPercentage = 1f;
            }
        }
    }
}
