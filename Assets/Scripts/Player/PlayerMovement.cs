using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool canMove = true;
    public Settings bind;
    public Crouch crouch;

    public float o_moveSpeed;
    public float o_jump;
    [Header("Movement")]
    public float moveSpeed;
    public float sprintSpeed;
    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpGravity;
    public float cooldown;
    private float o_cooldown;

    [Header("Ground Check")]
    public float playerHeight;
    public bool grounded;
    public bool can_jump;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        o_cooldown = cooldown;

        sprintSpeed = moveSpeed * 2f;
        o_moveSpeed = moveSpeed;
        o_jump = jumpForce;
    }

    private void Update()
    {
        if(!canMove)
        {
            return;
        }
        
        MyInput();
        SpeedControl();
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f);

        if(grounded){
            rb.drag = groundDrag;
        }
        else{
            rb.drag = 0;
        }
        if(Input.GetKeyDown(bind.jump) && grounded)
        {
            Jump();
        }
        if(o_cooldown > 0 && !can_jump)
        {
            o_cooldown -= Time.deltaTime;
        }
        else if(o_cooldown <= 0 && !can_jump)
        {
            o_cooldown = cooldown;
            can_jump = true;
        }
    }

    private void FixedUpdate() 
    {
        MovePlayer();
        ApplyCustomGravity();
    }

    private void ApplyCustomGravity()
    {
        if (!grounded && rb.velocity.y < .8f) // Apply custom gravity only when not grounded and falling
        {
            Vector3 extraGravityForce = (Physics.gravity * (jumpGravity - 1)) - Physics.gravity;
            rb.AddForce(extraGravityForce, ForceMode.Acceleration);
        }
    }
    

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void Jump()
    {
        can_jump = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Clear vertical velocity
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        rb.AddForce(Physics.gravity * (jumpGravity - 1), ForceMode.Acceleration); // Apply custom gravity
    }

    private void MovePlayer()
    {

        if(Input.GetKey(bind.sprint) && !crouch.is_crouching)
        {
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
            rb.AddForce(moveDirection.normalized * sprintSpeed * 10f);
        }
        else
        {
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f);
        }

    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
}