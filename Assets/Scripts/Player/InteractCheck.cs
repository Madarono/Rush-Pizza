using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Checks
{
    public string tagName;
    public GameObject interactionVisual;
    public bool customer;
    public bool merchant;
}

public class InteractCheck : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Rigidbody player;
    public Pausing pausing;
    public Settings settings;
    public LayerMask checkLayers;
    public Checks[] checks;
    public float extraRange = 1f;
    private Customer customer;
    private Merchant merchant;

    void Update()
    {
        CheckForInteractable();

        if(Input.GetMouseButtonDown(0))
        {
            if(customer != null && customer.state == States.Static)
            {
                pausing.lockMouse = false;
                customer.InitiateTalk(TalkType.Initial);
                playerMovement.canMove = false;
                player.velocity = Vector3.zero;
               
                customer = null;
            }
            else if(merchant != null && merchant.state == MerchantStates.Static)
            {
                pausing.lockMouse = false;
                merchant.InitiateTalk();
                playerMovement.canMove = false;
                player.velocity = Vector3.zero;
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

            foreach(Checks check in checks)
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
                    else if(check.merchant)
                    {
                        merchant = obj.GetComponent<Merchant>();
                        if(merchant.state != MerchantStates.Static)
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
                check.interactionVisual.SetActive(false);
            }
            customer = null;
            merchant = null;
        }
    }
}
