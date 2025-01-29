using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pausing : MonoBehaviour
{
    public MouseCursor mouse;
    public Settings settings;
    public HoverInformaton[] buttons;
    public GameObject pauseWindow;
    public Animator pauseAnimator;
    public float delayOfLeaving = 0.9f;
    private bool isPausing = false;

    void Start()
    {
        pauseWindow.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(settings.pause))
        {
            StopAllCoroutines();
            if(!isPausing)
            {
                Pause();
            }
            else
            {
                UnPause();
            }
            isPausing = !isPausing;
        }
    }

    void Pause()
    {
        mouse.FreeCusorState();
        Time.timeScale = 0f;
        pauseWindow.SetActive(true);
        StartCoroutine(DelayMoving());
    }

    void UnPause()
    {
        mouse.LockCusorState();
        Time.timeScale = 1f;
        StartCoroutine(WaitForPauseClosing());
        StartCoroutine(DelayMoving());
    }

    IEnumerator WaitForPauseClosing()
    {
        pauseAnimator.SetTrigger("Unpause");
        yield return new WaitForSeconds(delayOfLeaving);
        pauseWindow.SetActive(false);
    }

    IEnumerator DelayMoving()
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].canMove = false;
        }

        yield return new WaitForSecondsRealtime(delayOfLeaving);

        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].canMove = true;
        }
    }
}
