using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyer : MonoBehaviour
{
    public Vector3 direction = new Vector3(1, 0, 0);
    public float speed = 2f;
    private Pizza pizza;
    public Transform referenceX;

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        pizza = other.GetComponent<Pizza>();
        if (rb != null && pizza != null)
        {
            Vector3 movement = direction.normalized * speed * Time.deltaTime;
            rb.MovePosition(rb.position + movement);
        }
    }

    private void OnTriggerEnd(Collider other)
    {
        pizza = other.GetComponent<Pizza>();
        if(pizza != null)
        {
            pizza.canBeCooked = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        pizza = other.GetComponent<Pizza>();
        if (rb != null && pizza != null)
        {
            pizza.dragAndDrop.DropObject();

            rb.velocity = Vector3.zero;

            other.transform.position = new Vector3(other.transform.position.x, other.transform.position.y, referenceX.position.z);
            other.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        }
    }
}
