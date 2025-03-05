using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pausing : MonoBehaviour
{
    [Header("Scripts")]
    public MouseCursor mouse;
    public Settings settings;
    public Controls controls;
    public RecipeSystem recipeSys;
    public Tabs tabs;
    public Brief brief;

    [Header("Pausing")]
    public HoverInformaton[] buttons;
    public GameObject pauseWindow;
    public Animator pauseAnimator;
    public float delayOfLeaving = 0.9f;
    private bool isPausing = false;
    public bool lockMouse = true;
    public bool canPause = true; //Controls.cs controls this.

    void Start()
    {
        pauseWindow.SetActive(false);
        canPause = true;
    }

    void Update()
    {
        if(!canPause)
        {
            return;
        }

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
        brief.CloseWindow();
        StartCoroutine(DelayMoving());
    }

    void UnPause()
    {
        if(lockMouse)
        {
            mouse.LockCusorState();
        }
        Time.timeScale = 1f;
        controls.CloseWindow();
        recipeSys.CloseWindow();
        tabs.ResetAllTabs();
        StartCoroutine(WaitForPauseClosing());
        StartCoroutine(DelayMoving());
    }

    IEnumerator WaitForPauseClosing()
    {
        pauseAnimator.SetTrigger("Close");
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
