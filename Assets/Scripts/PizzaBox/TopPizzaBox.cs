using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class TopPizzaBox : MonoBehaviour
{
    public SoundManager sound;
    public PizzaChecker pizzaChecker;

    void Start()
    {
        if(sound == null)
        {
            sound = GameObject.Find("UniversalScripts").GetComponent<SoundManager>();
        }
    }
    
    public void notifyPizzaChecker()
    {
        sound.GenerateSound(transform.position, sound.closePizzaBox, true, .35f);
        pizzaChecker.KillPizza();
    } 
}