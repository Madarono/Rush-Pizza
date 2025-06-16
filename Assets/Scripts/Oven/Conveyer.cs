using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyer : MonoBehaviour
{
    public SoundManager sound;
    public Tutorial tutorial;
    public Vector3 direction = new Vector3(1, 0, 0);
    public float speed = 2f;
    private Pizza pizza;
    public Transform referenceX;

    [HideInInspector]public List<Pizza> pizzaOnConveyer = new List<Pizza>();
    private GameObject cacheLastSound;
    [HideInInspector]public AudioSource cacheLastSource;

    void Update()
    {
        if(pizzaOnConveyer.Count == 0 && cacheLastSound != null)
        {
            DeleteSound();
        }
    }

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
            pizza.dragAndDrop.DropObject(false);

            rb.velocity = Vector3.zero;

            other.transform.position = new Vector3(other.transform.position.x, other.transform.position.y, referenceX.position.z);
            other.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);

            if(!pizzaOnConveyer.Contains(pizza))
            {
                if(pizzaOnConveyer.Count == 0)
                {
                    sound.GenerateSound(transform.position, sound.conveyerBelt, true, 0.06f);
                    cacheLastSound = sound.lastSound;
                    cacheLastSource = sound.lastScript;
                }
                pizzaOnConveyer.Add(pizza);
            }

            if(tutorial.states == TutorialStates.CookPizza)
            {
                tutorial.hasCooked = true;
                tutorial.CheckRequirements();
            }
        }
    }

    void DeleteSound()
    {
        if(sound.lastSound == this.cacheLastSound)
        {
            sound.DeleteLastSound();
        }
        else
        {
            Destroy(cacheLastSound);
        }
        cacheLastSource = null;
    }

    public void CheckForPizza(Pizza pizza)
    {
        if(pizzaOnConveyer.Contains(pizza))
        {
            pizzaOnConveyer.Remove(pizza);
        }
    }
}
