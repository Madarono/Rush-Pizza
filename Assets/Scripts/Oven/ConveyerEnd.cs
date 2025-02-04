using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerEnd : MonoBehaviour
{
    [Header("End of conveyer")]
    public bool checkPickable = false;
    
    [Header("Cooking")]
    public bool checkCooked = false;

    [Header("Gates")]
    public bool isGate;
    public bool openGate;
    public GameObject gate;


    private Pickable pickable;

    private void OnTriggerStay(Collider other)
    {
        Pizza pizza = other.GetComponent<Pizza>();
        if(pizza != null)
        {
            if(checkPickable)
            {
                pickable = other.GetComponent<Pickable>();
                pickable.canBePicked = true;
                pizza.canBeCooked = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Pizza pizza = other.GetComponent<Pizza>();
        if(pizza != null)
        {
            if(checkCooked)
            {
                pizza.isCooked = true;
                pizza.cookedTimes++;
                pizza.canBeCooked = false;
                pizza.UpdateLooks();
            }

            pickable = other.GetComponent<Pickable>();
            if(isGate && pickable != null && !pickable.canBePicked)
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
        }
    }
}