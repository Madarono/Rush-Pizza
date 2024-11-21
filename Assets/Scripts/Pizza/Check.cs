using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Check : MonoBehaviour
{
    public Settings settings;
    public SauceDrawing drawing;
    public ToppingBox topping;

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
                }
                ToppingBox box = obj.GetComponent<ToppingBox>();
                drawing.topping = box.topping;
                topping = box;
                topping.isUsed = true;
                return;
            }
            else
            {
                if(topping != null)
                {
                    topping.isUsed = false;
                    drawing.topping = null;
                }
            }
        }
        else
        {
            if(topping != null)
            {
                topping.isUsed = false;
                drawing.topping = null;
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
                        drawing.topping = null;
                    }
                }
            }
        }
    }
}