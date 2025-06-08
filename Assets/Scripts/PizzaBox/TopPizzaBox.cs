using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class TopPizzaBox : MonoBehaviour
{
    public SoundManager sound;
    public PizzaChecker pizzaChecker;
    public Tutorial tutorial;

    void Start()
    {
        if(sound == null)
        {
            sound = GameObject.Find("UniversalScripts").GetComponent<SoundManager>();
        }
        if(tutorial == null)
        {
            tutorial = GameObject.Find("UniversalScripts").GetComponent<Tutorial>();
        }
    }
    
    public void notifyPizzaChecker()
    {
        sound.GenerateSound(transform.position, sound.closePizzaBox, true, .35f);
        pizzaChecker.KillPizza();

        if(tutorial == null || tutorial.hasCompleted)
        {
            return;
        }

        if(tutorial.states == TutorialStates.DeliverPizza)
        {
            tutorial.hasDelivered = true;
            tutorial.CheckRequirements();
        }
    } 
}