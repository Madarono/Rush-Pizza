using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaChecker : MonoBehaviour
{
    public PizzaBox box;
    public float checkRange;
    public bool pizzaOnTop;
    public GameObject pizza;
    public LayerMask pizzaLayer;
    public float pizzaKillDuration = 0.35f;

    void Update()
    {
        CheckPizza();
    }

    void CheckPizza()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, Vector3.up, out hit, checkRange, pizzaLayer))
        {
            pizzaOnTop = true;
            pizza = hit.collider.gameObject;
        }
        else
        {
            pizzaOnTop = false;
            pizza = null;
        }
    }

    public void KillPizza()
    {
        StartCoroutine(TimeToKillPizza());
    }

    IEnumerator TimeToKillPizza()
    {
        yield return new WaitForSeconds(pizzaKillDuration);
        box.GetPizzaInformation(pizza);
        Destroy(pizza);
    }
}
