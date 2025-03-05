using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Check : MonoBehaviour
{
    public Settings settings;
    public SauceDrawing drawing;
    public ToppingBox topping;

    private PizzaCutter cutter;

    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            CheckObject();
        }

        if(Input.GetMouseButtonDown(1))
        {
            InteractObject();
        } 
    }

    public void CheckObject()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.forward, out hit, settings.lookRange))
        {
            GameObject obj = hit.collider.gameObject;
            if(obj.CompareTag("ToppingBox"))
            {
                if(topping != null)
                {
                    topping.isUsed = false;
                    topping.CloseUseBox();
                }
                ToppingBox box = obj.GetComponent<ToppingBox>();
                drawing.topping = box.topping;
                drawing.idForSupply = box.id;
                topping = box;
                topping.isUsed = true;
                topping.OpenUseBox();
                return;
            }
            else if(obj.CompareTag("PizzaCutter"))
            {
                if(cutter == null)
                {
                    cutter = obj.GetComponent<PizzaCutter>();
                }

                if(cutter.isUsed)
                {
                    cutter.TurnOff();
                }
                else
                {
                    cutter.TurnOn(transform);
                }
            }
            else if(obj.CompareTag("PizzaBox"))
            {
                Animator anim = obj.GetComponent<Animator>();
                TopPizzaBox top = obj.GetComponent<TopPizzaBox>();
                if(top.pizzaChecker.pizza != null)
                {
                    top.notifyPizzaChecker();
                    anim.SetBool("Close", true);
                }
            }
            else
            {
                if(topping != null)
                {
                    topping.isUsed = false;
                    topping.CloseUseBox();
                    drawing.topping = null;
                }
                if(cutter != null)
                {
                    cutter.TurnOff();
                }
            }
        }
        else
        {
            if(topping != null)
            {
                topping.isUsed = false;
                topping.CloseUseBox();
                drawing.topping = null;
            }
            if(cutter != null)
            {
                cutter.TurnOff();
            }
        }
    }

    public void InteractObject()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.forward, out hit, settings.lookRange))
        {
            GameObject obj = hit.collider.gameObject;

            if(obj.CompareTag("Draggable"))
            {
                Interactable interactable = obj.GetComponent<Interactable>();
                if(interactable != null)
                {
                    interactable.Spawn();
                    if(topping != null)
                    {
                        topping.isUsed = false;
                        topping.CloseUseBox();
                        drawing.topping = null;
                    }
                }
            }
        }
    }
}