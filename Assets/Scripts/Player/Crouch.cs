using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crouch : MonoBehaviour
{
    public PlayerMovement movement;
    public Settings binds;
    private Rigidbody rb;
    public DragAndDrop dragAndDrop;
    public float crouch_speed;
    public bool can_move = true;
    public int debug_crouch = 0;
    public bool done = false;
    public float downForce;
    public float standingScale;
    public float crouchingScale;

    public bool hold; //if yes then make it hold, if no then make it toggle
    public bool is_crouching;
    public KeyCode crouch_key = KeyCode.C;

    void Start()
    {
        // o_speed = movement.o_moveSpeed;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        crouch_key = binds.crouch;
        hold = binds.hold_crouch;
        if(can_move)
        {
            if((Input.GetKey(crouch_key) && hold))
            {
                if(dragAndDrop.heldObject != null)
                {
                    dragAndDrop.DropObject();
                }
                _Crouch();
            }
            else if((Input.GetKeyUp(crouch_key) && hold))
            {
                Stand();
            }
    
            
            if(Input.GetKeyDown(crouch_key) && !hold && !is_crouching)
            {
                if(dragAndDrop.heldObject != null)
                {
                    dragAndDrop.DropObject();
                }
                _Crouch();
                Vector3 force = transform.up * downForce * -1;
                rb.AddForce(force, ForceMode.Impulse);
            }
            else if((Input.GetKeyDown(crouch_key) && !hold && is_crouching))
            {
                Stand();
            }
    
            if(is_crouching && debug_crouch == 1 && !done && hold)
            {
                Vector3 force = transform.up * downForce * -1;
                rb.AddForce(force, ForceMode.Impulse);
                debug_crouch = 0;
                done = true;
            }

            if(is_crouching)
            {
                movement.moveSpeed = crouch_speed;
            }
            else
            {
                movement.moveSpeed = movement.o_moveSpeed;
            }
        }
    }

    void Stand()
    {
        transform.localScale = new Vector3(transform.localScale.x, standingScale, transform.localScale.z);
        is_crouching = false;
        done = false;
    }

    void _Crouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, crouchingScale, transform.localScale.z);
        is_crouching = true;
        debug_crouch = 1;
    }
}
