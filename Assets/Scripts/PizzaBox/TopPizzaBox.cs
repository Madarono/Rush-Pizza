using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class TopPizzaBox : MonoBehaviour
{
    public PizzaChecker pizzaChecker;
    
    public void notifyPizzaChecker()
    {
        pizzaChecker.KillPizza();
    } 
}