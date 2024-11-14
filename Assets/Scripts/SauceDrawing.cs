using System.Collections.Generic;
using UnityEngine;

public class SauceDrawing : MonoBehaviour
{
    public Settings settings;
    public DragAndDrop dragAndDrop;
    public ToppingSO topping;
    
    private List<Vector3> drawnPath = new List<Vector3>();
    private bool isDrawing;
    private Transform camera;
    private Vector3 lastSpawnPosition;  // Tracks the last spawn position to check distance

    void Start()
    {
        camera = transform;
    }

    void Update()
    {
        if(topping == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1) && dragAndDrop.heldObject == null)
        {
            if(!topping.hold)
            {
                DrawSauce();
            }
            else
            {
                isDrawing = true;
            }
            lastSpawnPosition = Vector3.positiveInfinity;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isDrawing = false;
        }

        if (isDrawing)
        {
            DrawSauce();
        }
    }

    void DrawSauce()
    {
        if (Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, settings.lookRange, topping.drawingSurfaceLayer))
        {
            Vector3 hitPoint = hit.point;
    
            // Only spawn sauce if far enough from last position and no other sauce nearby
            if (Vector3.Distance(hitPoint, lastSpawnPosition) > topping.minDistance && 
                ((topping.checkToppinglayer && !SauceNearby(hitPoint)) || !topping.checkToppinglayer))
            {
                drawnPath.Add(hitPoint);
    
                // Instantiate the sauce object at the calculated position
                GameObject sauce = Instantiate(topping.toppingPrefab, hitPoint + Vector3.up * topping.spawnOffset, Quaternion.identity);
    
                // Calculate random rotations for each axis if enabled in `rotationRandom`
                float xRotation = topping.rotationRandom[0] ? Random.Range(-360f, 360f) : 0f;
                float yRotation = topping.rotationRandom[1] ? Random.Range(-360f, 360f) : 0f;
                float zRotation = topping.rotationRandom[2] ? Random.Range(-360f, 360f) : 0f;
    
                // Apply both random rotations and offsets for each axis
                sauce.transform.rotation = Quaternion.Euler(
                    xRotation + topping.rotationOffsets[0],
                    yRotation + topping.rotationOffsets[1],
                    zRotation + topping.rotationOffsets[2]
                );
    
                // Set the parent to the hit object for proper hierarchy
                sauce.transform.SetParent(hit.transform);
                Pizza pizzaScript = sauce.GetComponentInParent<Pizza>();
                pizzaScript.CheckName(topping.name, sauce);
                lastSpawnPosition = hitPoint;  // Update the last spawn position
            }
        }
    }


    bool SauceNearby(Vector3 position)
    {
        // Check for sauce objects within a specified radius
        Collider[] colliders = Physics.OverlapSphere(position, topping.spawnRadius, topping.toppingLayer);
        return colliders.Length > 0;
    }
}
