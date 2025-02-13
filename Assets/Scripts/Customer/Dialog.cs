using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum PizzaOptions
{
    Sauce,
    Cheese,
    Pepperoni,
    Sausage,
    Mushroom,
    Olive,
    Onion,
    Paprika
}

public enum TalkType
{
    Initial,
    What,
    Hint
}

public enum PizzaCook
{
    Raw,
    Cooked,
    DoubleCooked,
    TripleCooked
}

[System.Serializable]
public class Pizzas
{
    public PizzaOptions[] leftToppings;
    public PizzaOptions[] rightToppings;
    public PizzaCook cookTimes;
    public int numberOfCuts = 3;
    public int minimumCutsAllowed = 3;
    public int maximumCutsAllowed = 5;
}

[System.Serializable]
public class Talk
{
    public string content;
    public string contentDeutsch;
    public TalkType type;
}

[CreateAssetMenu(fileName="Dialog", menuName="Custom/Dialog", order=1)]
public class Dialog : ScriptableObject
{
    public Talk[] talk = new Talk[3];
    public float speedOfTalk = 0.4f;

    public Pizzas[] pizzas;

    [Header("Other Toppings")]
    public PizzaOptions[] hatedToppings;
    public int maximumToppingsAllowed = 3;

    [Header("Recipe System")]
    public bool giveToRecipe = true;
    public int indexOfRecipe;
}