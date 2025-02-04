using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameObject interactPrefab;
    public Vector3 prefabRotation;
    public Settings settings;
    public Stats stats;
    public DragAndDrop dragAndDrop;
    public float costToChange = 2f;

    public void Spawn()
    {
        gameObject.SetActive(false);
        GameObject go = Instantiate(interactPrefab, transform.position, Quaternion.identity);
        go.transform.rotation = Quaternion.Euler(prefabRotation.x, prefabRotation.y, prefabRotation.z);

        if(dragAndDrop != null)
        {
            Pickable pickable = go.GetComponent<Pickable>();
            pickable.dragAndDrop = dragAndDrop;
        }
       
        if(settings != null)
        {
            settings.AddWithoutVisual(-costToChange);
            stats.doughSpent += costToChange;
        }

        Destroy(gameObject);
    }
}