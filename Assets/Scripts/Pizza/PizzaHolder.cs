using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaHolder : MonoBehaviour
{
    public Pizza pizza;
    public GameObject visualCutter;

    void Start()
    {
        if(pizza.tutorial != null)
        {
            VisualCutter cutter = pizza.visualCutter.GetComponent<VisualCutter>();
            cutter.tutorial = pizza.tutorial;
        }
    }
}
