using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintFocus : MonoBehaviour
{
    public Player_Cam playerCam;
    public Camera camera;
    public Controls controls;
    public float currentFov;
    public float increamentFov = 10f;
    public float speedToIncreament = 2f;
    public float speedToDecreament = 4f;
    public bool focus;
    private float fullFov;
    private float originalFov;

    void Start()
    {
        currentFov = controls.currentFov;
        fullFov = controls.currentFov;
        originalFov = controls.currentFov;
    }

    void FixedUpdate()
    {
        if(focus)
        {
            currentFov = Mathf.Lerp(currentFov, fullFov, Time.deltaTime * speedToIncreament);
            camera.fieldOfView = currentFov;
        }
        else if(!focus && !playerCam.goZoom)
        {
            currentFov = Mathf.Lerp(currentFov, originalFov, Time.deltaTime * speedToDecreament);
            camera.fieldOfView = currentFov;
        }
    }

    public void DoFocus()
    {
        if(focus)
        {
            return;
        }

        originalFov = controls.currentFov;
        fullFov = currentFov + increamentFov;
        focus = true;
    }

    public void LoseFocus()
    {
        focus = false;
    }
}
