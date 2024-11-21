using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyer : MonoBehaviour
{
    public Vector3 direction = new Vector3(1, 0, 0);
    public float speed = 2f;

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        Pizza pizza = other.GetComponent<Pizza>();
        if (rb != null && pizza != null)
        {
            // otherObj = other.GameObject;
            Pickable pickable = other.GetComponent<Pickable>();
            pickable.canBePicked = false;
            pickable.dragAndDrop.DropObject();
            // Apply movement in the specified direction and speed
            Vector3 movement = direction.normalized * speed * Time.deltaTime;
            rb.MovePosition(rb.position + movement);
        }
    }
}
