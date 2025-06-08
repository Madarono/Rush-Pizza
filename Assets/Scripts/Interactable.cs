using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public SoundManager sound;
    public GameObject interactPrefab;
    public Vector3 prefabRotation;
    public Settings settings;
    public Stats stats;
    public DragAndDrop dragAndDrop;
    public Tutorial tutorial;
    public float costToChange = 2f;

    public void Spawn()
    {
        gameObject.SetActive(false);
        GameObject go = Instantiate(interactPrefab, transform.position, Quaternion.identity);
        go.transform.rotation = Quaternion.Euler(prefabRotation.x, prefabRotation.y, prefabRotation.z);

        if(dragAndDrop != null)
        {
            Pizza pizza = go.GetComponent<Pizza>();
            pizza.dragAndDrop = dragAndDrop;
            if(tutorial != null)
            {
                pizza.tutorial = this.tutorial;
            }
        }
       
        if(settings != null)
        {
            settings.AddWithoutVisual(-costToChange);
            stats.doughSpent += costToChange;
        }

        if(tutorial != null)
        {
            tutorial.hasUsed = true;
            tutorial.CheckRequirements();
        }

        sound.GenerateSound(transform.position, sound.spreadDough, true, .15f);

        Destroy(gameObject);
    }
}