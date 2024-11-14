using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 1f; // Adjust speed for more intuitive control
    public GameObject targetedObject;
    public Player_Cam p_cam;

    public bool invertY;
    public bool invertX;

    private Move_HoldPos move_holdPos;
    private DragAndDrop drag;
    private bool isRotating = false;
    private Vector3 o_obj;

    private Vector3 null_vec;
    
    void Start()
    {
        null_vec = new Vector3(1,1,1);
        move_holdPos = gameObject.GetComponent<Move_HoldPos>();
        drag = gameObject.GetComponent<DragAndDrop>();
    }

    void Update()
    {
        // targetedObject.transform.localScale = null_vec; 
        // Check for right mouse button press
        if (Input.GetMouseButtonDown(1) && targetedObject.transform.childCount > 0)
        {
            o_obj = move_holdPos.hold_pos.position;
            drag.heldObject.transform.SetParent(null);
            targetedObject.transform.position = drag.heldObject.transform.position;
            drag.heldObject.transform.SetParent(targetedObject.transform);
            isRotating = true;
            p_cam.canMove = false;
        }

        // Check for right mouse button release
        if (Input.GetMouseButtonUp(1))
        {
            isRotating = false;
            p_cam.canMove = true;
        }

        // if(isRotating)
        // {
        //     // drag.heldObject.transform.position = o_obj;
        // }

        // Rotate the targeted object if the right mouse button is held down
        if (isRotating && targetedObject != null)
        {
            // Get the mouse movement delta
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Create a rotation based on the mouse movement
            Quaternion rotationYQuaternion;
            Quaternion rotationXQuaternion;

            if(invertX)
            {
                rotationYQuaternion = Quaternion.Euler(0, mouseX * rotationSpeed * -1f, 0);
            }
            else
            {
                rotationYQuaternion = Quaternion.Euler(0, mouseX * rotationSpeed, 0);
            }

            if(invertY)
            {
                rotationXQuaternion = Quaternion.Euler(mouseY * rotationSpeed, 0, 0);
            }
            else
            {
                rotationXQuaternion = Quaternion.Euler(mouseY * rotationSpeed * -1f, 0, 0);
            }

            // Apply the rotations in world space
            targetedObject.transform.rotation = rotationYQuaternion * targetedObject.transform.rotation * rotationXQuaternion;
            if(drag.heldObject != null)
            {
                drag.heldObject.transform.SetParent(null);
                targetedObject.transform.position = drag.heldObject.transform.position;
                drag.heldObject.transform.SetParent(targetedObject.transform);
            }
        }
    }
}
