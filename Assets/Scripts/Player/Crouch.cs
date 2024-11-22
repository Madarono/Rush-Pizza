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

    [Header("Crouch Camera Position")]
    public Transform camera;
    public Transform standingPosition;
    public Transform crouchingPosition;
    public float transitionSpeed = 2f;

    [Header("Modifications")]
    public bool is_crouching;
    public bool can_move = true;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(!can_move)
        {
            return;
        }
            
        if((Input.GetKeyDown(binds.crouch) && binds.holdCrouch))
        {
            StartCrouch();
        }
        else if((Input.GetKeyUp(binds.crouch) && binds.holdCrouch))
        {
            StartStand();
        }
        
        if(Input.GetKeyDown(binds.crouch) && !binds.holdCrouch && !is_crouching)
        {
            StartCrouch();
        }
        else if((Input.GetKeyDown(binds.crouch) && !binds.holdCrouch && is_crouching))
        {
            StartStand();
        }
    }

    void StartStand()
    {
        is_crouching = false;
        movement.moveSpeed = movement.o_moveSpeed;
    }

    void StartCrouch()
    {
        is_crouching = true;
        movement.moveSpeed = crouch_speed;
    }

    void FixedUpdate()
    {
        if(is_crouching)
        {
            camera.position = Vector3.Lerp(camera.position, crouchingPosition.position, Time.deltaTime * transitionSpeed);
        }
        else
        {
            camera.position = Vector3.Lerp(camera.position, standingPosition.position, Time.deltaTime * transitionSpeed);
        }
    }
}
