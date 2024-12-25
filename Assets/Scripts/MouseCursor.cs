using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    public GameObject cursor;
    public float cursorSpeed;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursor.SetActive(true);
    }

    public void ChangeCusorState()
    {
        Cursor.visible = false;
        Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
    }
    
    public void FreeCusorState()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LockCusorState()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            Vector3 targetPosition = Input.mousePosition;
            cursor.transform.position = Vector3.Lerp(cursor.transform.position, targetPosition, cursorSpeed * Time.deltaTime);
        }
        else
        {
            cursor.transform.localPosition = Vector3.one;
        }
    }
}
