using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerEnd : MonoBehaviour
{
    [Header("End of conveyer")]
    public bool checkPickable = false;
    public Conveyer conveyer;

    [Header("Cooking")]
    public bool checkCooked = false;

    [Header("Gates")]
    public bool isGate;
    public bool openGate;
    public GameObject gate;


    [Header("Mod")]
    public SoundManager sounds;

    private void OnTriggerStay(Collider other)
    {
        Pizza pizza = other.GetComponent<Pizza>();
        if(pizza != null)
        {
            if(checkPickable)
            {
                // pickable = other.GetComponent<Pickable>();
                // pickable.canBePicked = true
                conveyer.CheckForPizza(pizza);
                pizza.canBeCooked = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Pizza pizza = other.GetComponent<Pizza>();
        if(pizza != null)
        {
            if(checkCooked && conveyer.pizzaOnConveyer.Contains(pizza))
            {
                pizza.isCooked = true;
                pizza.cookedTimes++;
                pizza.canBeCooked = false;
                pizza.UpdateLooks();
            }

            if(isGate)
            {
                if(openGate)
                {
                    gate.SetActive(false);
                }
                else
                {
                    gate.SetActive(true);
                }
            }

            if(sounds != null)
            {
                sounds.Generate2DSound(transform.position, sounds.conveyerEnd, true, 1f);
            }
        }
    }
}