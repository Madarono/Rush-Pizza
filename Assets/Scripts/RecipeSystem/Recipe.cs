using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Custom/Recipe")]
public class Recipe : ScriptableObject
{
    public PizzaOptions[] toppings;
    public bool sauce;
    public bool cheese; 
    public string englishName;
    public string deutschName;
}