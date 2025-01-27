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
    public PizzaTopping[] leftToppings;
    public PizzaTopping[] rightToppings;
    public PizzaCook cookTimes;
    public int numberOfCuts = 3;
    public int minimumCutsAllowed = 3;
    public int maximumCutsAllowed = 5;
}

[System.Serializable]
public class PizzaTopping
{
    public PizzaOptions topping;
    public int amount;
    public int minDifferenceAccepted;
    public int maxDifferenceAccepted;
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
    public Talk[] talk;
    public float speedOfTalk = 0.2f;

    public Pizzas[] pizzas;
}