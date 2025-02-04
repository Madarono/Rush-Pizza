using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowOpening : MonoBehaviour
{
    public GameObject header;
    public GameObject window;
    
    [Header("Animations")]
    public Animator headerAnim;
    public Animator windowAnim;
    public float durationForClosing = 1f;

    public bool isOpen = false;

    public virtual void Start()
    {
        if(header != null)
        {
            header.SetActive(false);
        }

        if(window != null)
        {
            window.SetActive(false);
        }
    }

    public virtual void BothWindow()
    {
        if(isOpen)
        {
            CloseWindow();
        }
        else
        {
            OpenWindow();
        }
    }

    public virtual void CloseWindow()
    {
        StartCoroutine(ClosingWindow());
    }

    IEnumerator ClosingWindow()
    {
        if(headerAnim != null)
        {
            headerAnim.SetTrigger("Close");
        }
        if(windowAnim != null)
        {
            windowAnim.SetTrigger("Close");
        }
        
        yield return new WaitForSecondsRealtime(durationForClosing);
        
        if(header != null)
        {
            header.SetActive(false);
        }
        if(window != null)
        {
            window.SetActive(false);
        }

        isOpen = false;
    }
    
    public virtual void OpenWindow()
    {
        if(header != null)
        {
            header.SetActive(true);
        }

        if(window != null)
        {
            window.SetActive(true);
        }
        isOpen = true;
    }
}
