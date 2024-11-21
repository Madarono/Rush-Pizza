using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ingrediants
{
    public string ingrediantName;
    public List<GameObject> ingrediantObj;
    public float maxIngrediantPercentage = 100f;
    public float ingrediantPercentage;

    [Header("Side of the toppping")]
    public List<GameObject> leftSideObj = new List<GameObject>();
    public float leftPercentage;
    public List<GameObject> rightSideObj = new List<GameObject>();
    public float rightPercentage;

    [Header("Separation of Toppings")]
    public bool calculateDistance = true;
    public List<float> distanceBetweenObjects = new List<float>();
    public float averageDistance; 
}

public class Pizza : MonoBehaviour
{
    public Ingrediants[] ingrediants;
    private PlayerHolder playerHolder;
    public GameObject sideSeparator; //Separates the left and right sides

    void Start()
    {
        playerHolder = GameObject.Find("UniversalScripts").GetComponent<PlayerHolder>();
    }

    void Update()
    {
        if(playerHolder.drawing.topping != null)
        {
            sideSeparator.SetActive(true);
        }
        else
        {
            sideSeparator.SetActive(false);
        }
    }

    public void CheckName(string name, GameObject obj)
    {
        for (int i = 0; i < ingrediants.Length; i++)
        {
            if (name == ingrediants[i].ingrediantName)
            {
                string side = CheckSide(obj.transform.position);
                if (side == "left")
                {
                    ingrediants[i].leftSideObj.Add(obj);
                }
                else if (side == "right")
                {
                    ingrediants[i].rightSideObj.Add(obj);
                }

                ingrediants[i].ingrediantObj.Add(obj);
                UpdatePercentage(i);
                if(ingrediants[i].calculateDistance)
                {
                    UpdateDistance(i);
                }
                break;
            }
        }
    }

    public void UpdatePercentage(int id)
    {
        ingrediants[id].ingrediantPercentage = ingrediants[id].ingrediantObj.Count / ingrediants[id].maxIngrediantPercentage;
        if (ingrediants[id].ingrediantPercentage > 1f)
        {
            ingrediants[id].ingrediantPercentage = 1f;
        }

        ingrediants[id].leftPercentage = ingrediants[id].leftSideObj.Count / (ingrediants[id].maxIngrediantPercentage / 2f);
        if(ingrediants[id].leftPercentage > 1f)
        {
            ingrediants[id].leftPercentage = 1f;
        }

        ingrediants[id].rightPercentage = ingrediants[id].rightSideObj.Count / (ingrediants[id].maxIngrediantPercentage / 2f);
        if(ingrediants[id].rightPercentage > 1f)
        {
            ingrediants[id].rightPercentage = 1f;
        }
    }

    public void UpdateDistance(int id)
    {
        // If there's only one ingredient object, distances cannot be calculated
        if (ingrediants[id].ingrediantObj.Count <= 1)
        {
            ingrediants[id].distanceBetweenObjects.Clear();
            ingrediants[id].averageDistance = 0f;
            return;
        }

        // Ensure the distanceBetweenObjects list matches the count of ingredient objects - 1
        while (ingrediants[id].distanceBetweenObjects.Count < ingrediants[id].ingrediantObj.Count - 1)
        {
            ingrediants[id].distanceBetweenObjects.Add(0);
        }
        while (ingrediants[id].distanceBetweenObjects.Count > ingrediants[id].ingrediantObj.Count - 1)
        {
            ingrediants[id].distanceBetweenObjects.RemoveAt(ingrediants[id].distanceBetweenObjects.Count - 1);
        }

        float totalDistance = 0f;

        for (int o = 0; o < ingrediants[id].ingrediantObj.Count - 1; o++)
        {
            // Calculate the distance between consecutive objects
            float distance = Vector3.Distance(
                ingrediants[id].ingrediantObj[o].transform.position,
                ingrediants[id].ingrediantObj[o + 1].transform.position
            );
            ingrediants[id].distanceBetweenObjects[o] = distance;
            totalDistance += distance;
        }

        // Calculate the average distance
        ingrediants[id].averageDistance = totalDistance / (ingrediants[id].ingrediantObj.Count - 1);
    }


    private string CheckSide(Vector3 ingredientPosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(ingredientPosition);
        return localPosition.x < 0 ? "left" : "right";
    }
}
