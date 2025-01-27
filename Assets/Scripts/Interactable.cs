using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameObject interactPrefab;
    public Vector3 prefabRotation;
    public Settings settings;
    public float costToChange = 5f;

    public void Spawn()
    {
        gameObject.SetActive(false);
        GameObject go = Instantiate(interactPrefab, transform.position, Quaternion.identity);
        go.transform.rotation = Quaternion.Euler(prefabRotation.x, prefabRotation.y, prefabRotation.z);
       
        if(settings != null)
        {
            settings.AddWithoutVisual(-costToChange);
        }

        Destroy(gameObject);
    }
}