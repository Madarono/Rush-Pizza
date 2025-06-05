using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Cam : MonoBehaviour
{
    public float sensX;
    public float sensY;
    public Transform orientation;
    float xRotation;
    float yRotation;

    [Header("Zoom")]
    public Controls controls;
    public SprintFocus sprintFocus;
    public Settings settings;
    public Camera camera;
    public float zoomSpeed = 5f;
    public bool goZoom = false;
    public float targetZoom;
    public float zoomMultiplyer = 0.6f;
    
    
    public bool canMove = true;

    private void Update()
    {
        if(!canMove)
        {
            return;
        }

        if(Input.GetKeyDown(settings.zoom))
        {
            ZoomIn();
        }
        else if(Input.GetKeyUp(settings.zoom))
        {
            ZoomOut();
        }
        
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        if(camera.fieldOfView >= controls.currentFov && !goZoom)
        {
            sprintFocus.enabled = true;
        }
    }

    private void FixedUpdate()
    {
        if(goZoom)
        {
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetZoom, zoomSpeed * Time.deltaTime);
        }
        else
        {
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, controls.currentFov, zoomSpeed * Time.deltaTime);
        }
    }

    void ZoomIn()
    {
        CalculateTargetZoom();
        goZoom = true;
        sprintFocus.enabled = false;
    }

    void CalculateTargetZoom()
    {
        targetZoom = controls.currentFov * zoomMultiplyer;
    }

    void ZoomOut()
    {
        CalculateTargetZoom();
        goZoom = false;
    }
}
