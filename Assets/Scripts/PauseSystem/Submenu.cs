using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Submenu : MonoBehaviour
{
    public GameObject[] subTypes;
    public Animator[] subAnimator;
    public float closeDuration;
    public bool isOn = false;
    
    [Header("Disable Others")]
    public Submenu[] removeScripts;

    [Header("Visual")]
    public TextMeshProUGUI visual;

    void Start()
    {
        TurnOff();
    }
    

    public void ToggleVisibility()
    {
        isOn = !isOn;

        if(isOn)
        {
            TurnOff();
        }
        else
        {
            TurnOn();
        }
    }

    public void TurnOn()
    {
        foreach(GameObject obj in subTypes)
        {
            obj.SetActive(true);
        }
        
        if(removeScripts.Length > 0)
        {
            foreach(Submenu script in removeScripts)
            {
                script.isOn = false;
                script.TurnOff();
            }
        }

        if(visual != null)
        {
            visual.text = "-";
        }
    }

    public void TurnOff()
    {
        StartCoroutine(CloseAnimation());

        if(visual != null)
        {
            visual.text = "+";
        }
    }

    IEnumerator CloseAnimation()
    {
        foreach(Animator anim in subAnimator)
        {
            anim.SetTrigger("Close");
        }

        yield return new WaitForSeconds(closeDuration);
        
        foreach(GameObject obj in subTypes)
        {
            obj.SetActive(false);
        }
    }
}
