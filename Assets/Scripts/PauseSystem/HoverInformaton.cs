using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HoverInformaton : MonoBehaviour
{
    public Settings settings;
    public TextMeshProUGUI visual;
    public Transform[] statePositions;
    public float movingSpeed = 0.5f;
    [HideInInspector]public bool showingVisual = false;
    public bool canMove = true;
    public string english;
    public string deutsch;

    public void showVisual()
    {
        showingVisual = true;
        if(settings.english)
        {
            visual.text = english;
        }
        else
        {
            visual.text = deutsch;
        }

        visual.gameObject.SetActive(true);
    } 

    public void hideVisual()
    {
        showingVisual = false;
        visual.gameObject.SetActive(false);
    } 

    void Update()
    {
        if(!canMove)
        {
            return;
        }

        if(showingVisual)
        {
            transform.position = Vector3.Lerp(transform.position, statePositions[1].position, Time.unscaledDeltaTime * movingSpeed);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, statePositions[0].position, Time.unscaledDeltaTime * movingSpeed);
        }
    }
}
