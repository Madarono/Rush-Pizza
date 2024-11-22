using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DragDirection
{
    None,
    X,
    Y,
    Z
}
public class DragAndDrop : MonoBehaviour
{
    public Transform parent_obj;
    public Crouch crouch;
    private Move_HoldPos holdPos;
    public Settings settings;
    public LayerMask _ignore;
    public bool can_drag = true;
    public RotateObject rotate_obj;
    public float dragSpeed = 10f;
    public Transform holdPosition;

    public GameObject heldObject;
    private Rigidbody heldObjectRB;

    private bool isRotating = false;
    private Vector3 lastMousePosition;

    //[Header("Dragging by sliding")]
    public float dragForce = 10f;
    public float maxDragDistance = 5f;
    public DragDirection allowedDragDirection = DragDirection.X;

    private Camera mainCamera;
    private GameObject selectedObject;
    private Rigidbody selectedObjectRigidbody;
    private Vector3 initialPosition;
    private Vector3 offset;
    private float objectZCoord;
    private bool isDragging = false;

    void Start()
    {
        mainCamera = Camera.main;
        holdPos = gameObject.GetComponent<Move_HoldPos>();
    }

    void Update()
    {
        if (Input.GetKeyDown(settings.throwKey) && can_drag)
        {
            ThrowObject();
        }

        if(Input.GetMouseButtonDown(0) && can_drag)
        {
            if(heldObject == null)
            {
                TryPickupObject();
            }
            else
            {
                DropObject();
            }


            if(selectedObject == null)
            {
                TryPickupMoveable();
            }

        }

        if(Input.GetMouseButtonUp(0))
        {
            if(selectedObject != null)
            {
                DropMoveable();
            }
        }

        if(Input.GetMouseButtonDown(2) && can_drag)
        {
            if(selectedObject == null)
            {
                TryPickupMoveableDraggable();
            }
        }

        if(Input.GetMouseButtonUp(2))
        {
            if(selectedObject != null)
            {
                DropMoveable();
            }
        }

        if(heldObject != null)
        {
            MoveObject();
        }
    }

    void TryPickupObject()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, settings.lookRange, _ignore))
        {
            if(hit.collider.CompareTag("Draggable"))
            {
                // rotate_obj.targetedObject = heldObject;
                Pickable heldObjectScript = hit.collider.gameObject.GetComponent<Pickable>();
                if(heldObjectScript.canBePicked)
                {
                    heldObject = hit.collider.gameObject;
                    heldObjectScript.dragAndDrop = this;
                    heldObjectRB = heldObject.GetComponent<Rigidbody>();
                    heldObjectRB.constraints = RigidbodyConstraints.FreezeRotation;  
                    
                    heldObject.transform.SetParent(parent_obj);
    
                    holdPos.SetHoldPosition(2.5f);
                }
            }
            if(heldObjectRB != null)
            {
                heldObjectRB.useGravity = false;
                heldObjectRB.drag = 10;
            }
        }
    }

    public void DropObject()
    {
        if(heldObjectRB != null)
        {
            heldObjectRB.useGravity = true;
            heldObjectRB.drag = 0;
            
            heldObject.transform.position = holdPosition.position;
            heldObject.transform.SetParent(null);
            
            isDragging = false;
            heldObject = null;

            heldObjectRB.constraints = RigidbodyConstraints.None;  
            heldObjectRB = null;
        }
    }

    void MoveObject()
    {
        if(heldObjectRB != null)
        {
            Vector3 moveDirection = (holdPosition.position - heldObject.transform.position);
            heldObjectRB.velocity = moveDirection * dragSpeed;
        }
    }

    void ThrowObject()
    {
        if (heldObjectRB != null)
        {
            heldObjectRB.constraints = RigidbodyConstraints.None;
            Vector3 throwDirection = mainCamera.transform.forward;
            heldObjectRB.AddForce(throwDirection * settings.throwForce, ForceMode.Impulse);
            DropObject();
        }
    }

    void TryPickupMoveable()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);    
    
        if (Physics.Raycast(ray, out hit, settings.lookRange, _ignore))
        {
            if (hit.transform.CompareTag("Moveable"))
            {
                selectedObject = hit.transform.gameObject;
                selectedObjectRigidbody = selectedObject.GetComponent<Rigidbody>();    
    
                if (selectedObjectRigidbody != null)
                {
                    objectZCoord = mainCamera.WorldToScreenPoint(selectedObject.transform.position).z;
                    Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, objectZCoord);
                    offset = selectedObject.transform.position - mainCamera.ScreenToWorldPoint(mousePosition);
                    initialPosition = selectedObject.transform.position; // Store the initial position
    
                    isDragging = true;
                }
            }
        }
    }

    void TryPickupMoveableDraggable()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit, settings.lookRange, _ignore))
        {
            if(hit.transform.CompareTag("Draggable"))
            {
                selectedObject = hit.transform.gameObject;
                selectedObjectRigidbody = selectedObject.GetComponent<Rigidbody>();

                if(selectedObjectRigidbody != null)
                {
                    objectZCoord = mainCamera.WorldToScreenPoint(selectedObject.transform.position).z;
                    Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, objectZCoord);
                    offset = selectedObject.transform.position - mainCamera.ScreenToWorldPoint(mousePosition);
                    initialPosition = selectedObject.transform.position - mainCamera.ScreenToWorldPoint(mousePosition);
                    
                    selectedObjectRigidbody.constraints = RigidbodyConstraints.FreezeRotation;  
                    
                    // rotate_obj.targetedObject = selectedObject; 
                    
                    isDragging = true;
                }
            }
        }
    }

    void DropMoveable()
    {
        isDragging = false;
        selectedObject = null;
        selectedObjectRigidbody.constraints = RigidbodyConstraints.None;
        selectedObjectRigidbody = null;
        // rotate_obj.targetedObject = null; 
    }

    void FixedUpdate()
    {
        if (isDragging && selectedObjectRigidbody != null)
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, objectZCoord);
            Vector3 targetPosition = mainCamera.ScreenToWorldPoint(mousePosition) + offset;
    
            // Restrict movement to the allowed direction if it's not "None"
            if (allowedDragDirection != DragDirection.None)
            {
                switch (allowedDragDirection)
                {
                    case DragDirection.X:
                        targetPosition.y = initialPosition.y;
                        targetPosition.z = initialPosition.z;
                        break;
                    case DragDirection.Y:
                        targetPosition.x = initialPosition.x;
                        targetPosition.z = initialPosition.z;
                        break;
                    case DragDirection.Z:
                        targetPosition.x = initialPosition.x;
                        targetPosition.y = initialPosition.y;
                        break;
                }
            }
    
            // Calculate the distance from the initial position in the allowed direction
            float distanceFromInitial = Vector3.Distance(initialPosition, targetPosition);
    
            // If the distance exceeds the maximum, constrain the target position
            if (distanceFromInitial > maxDragDistance)
            {
                Vector3 direction = (targetPosition - initialPosition).normalized;
                if (allowedDragDirection != DragDirection.None)
                {
                    switch (allowedDragDirection)
                    {
                        case DragDirection.X:
                            direction.y = 0;
                            direction.z = 0;
                            break;
                        case DragDirection.Y:
                            direction.x = 0;
                            direction.z = 0;
                            break;
                        case DragDirection.Z:
                            direction.x = 0;
                            direction.y = 0;
                            break;
                    }
                }
                targetPosition = initialPosition + direction * maxDragDistance;
            }
    
            // Apply a smoother force to make the dragging movement more controlled
            Vector3 forceDirection = (targetPosition - selectedObjectRigidbody.position);
            selectedObjectRigidbody.velocity = forceDirection * dragForce * Time.fixedDeltaTime;
        }
    }

}
