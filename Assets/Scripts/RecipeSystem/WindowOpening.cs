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
        header.SetActive(false);
        window.SetActive(false);
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
        isOpen = false;
    }

    IEnumerator ClosingWindow()
    {
        headerAnim.SetTrigger("Close");
        windowAnim.SetTrigger("Close");
        yield return new WaitForSecondsRealtime(durationForClosing);
        header.SetActive(false);
        window.SetActive(false);
    }
    
    public virtual void OpenWindow()
    {
        header.SetActive(true);
        window.SetActive(true);
        isOpen = true;
    }
}
