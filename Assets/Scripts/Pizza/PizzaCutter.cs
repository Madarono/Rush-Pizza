using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaCutter : MonoBehaviour
{
    public bool isUsed = false;
    public bool isOnPizza = false;
    public Settings settings;
    public LayerMask pizzaLayer;
    private Transform cameraTransform;
    private PizzaHolder pizza;

    void Update()
    {
        if(isUsed)
        {
            CheckForPizza();
        }

    }

    public void TurnOn(Transform camera)
    {
        isUsed = true;
        cameraTransform = camera;

    }

    public void TurnOff()
    {
        isUsed = false;
        if(pizza != null)
        {
            pizza.visualCutter.SetActive(false);
        }
    }

    void CheckForPizza()
    {
        RaycastHit hit;

        if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, settings.lookRange, pizzaLayer))
        {
            pizza = hit.collider.gameObject.GetComponent<PizzaHolder>();
            if(pizza == null || !pizza.pizza.isCooked)
            {
                return;
            }

            pizza.visualCutter.SetActive(true);
            isOnPizza = true;
        }
        else
        {
            if(pizza != null)
            {
                pizza.visualCutter.SetActive(false);
                isOnPizza = false;
            }
        }
    }
}
