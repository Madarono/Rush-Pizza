using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Checks
{
    public string tagName;
    public GameObject interactionVisual;
    public bool customer;
}

public class InteractCheck : MonoBehaviour
{
    public Settings settings;
    public LayerMask checkLayers;
    public Checks[] checks;
    public float extraRange = 1f;
    private Customer customer;

    void Update()
    {
        CheckForInteractable();

        if(Input.GetMouseButtonDown(0))
        {
            if(customer != null && customer.state == States.Static)
            {
                customer.InitiateTalk(TalkType.Initial);
                customer = null;
            }
        }
    }

    void CheckForInteractable()
    {
        RaycastHit hit;

        // Debug.DrawRay(transform.position, transform.forward, Color.green, settings.lookRange);
        if(Physics.Raycast(transform.position, transform.forward, out hit, settings.lookRange + extraRange, checkLayers))
        {
            // Debug.Log("Found Something");
            GameObject obj = hit.collider.gameObject;

            foreach(var check in checks)
            {
                if(obj.CompareTag(check.tagName))
                {
                    // Debug.Log("Found Customer");
                    check.interactionVisual.SetActive(true);
                    
                    if(check.customer)
                    {
                        customer = obj.GetComponent<Customer>();
                        if(customer.state != States.Static)
                        {
                            check.interactionVisual.SetActive(false);
                        }
                    }

                    break;
                }
            }
        }
        else
        {
            foreach(var check in checks)
            {
                // Debug.Log("Removing Visuals");
                check.interactionVisual.SetActive(false);
            }
            customer = null;
        }
    }
}
