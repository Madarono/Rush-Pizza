using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_HoldPos : MonoBehaviour
{
    public Player_Cam cam;
    public Transform hold_pos;
    public float distanceToMove = 0.5f;

    public float minDistanceFromPlayer = 1f;
    public float maxDistanceFromPlayer = 10f;

    // Rotation variables
    // public float rotationSpeed = 5f; // Speed of rotation

    private void Update()
    {
        // Handle movement
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if ((scroll > 0))
        {
            MoveObjAwayFromPlayer();
        }

        if ((scroll < 0))
        {
            MoveObjTowardsPlayer();
        }
    }

    public void MoveObjTowardsPlayer()
    {
        Vector3 directionToPlayer = (Camera.main.transform.position - hold_pos.position).normalized;
        float currentDistance = Vector3.Distance(Camera.main.transform.position, hold_pos.position);
        if (currentDistance > minDistanceFromPlayer)
        {
            float distanceToMoveClamped = Mathf.Min(distanceToMove, currentDistance - minDistanceFromPlayer);
            hold_pos.position += directionToPlayer * distanceToMoveClamped;
        }
    }

    public void MoveObjAwayFromPlayer()
    {
        Vector3 directionToPlayer = (Camera.main.transform.position - hold_pos.position).normalized;
        float currentDistance = Vector3.Distance(Camera.main.transform.position, hold_pos.position);
        if (currentDistance < maxDistanceFromPlayer)
        {
            float distanceToMoveClamped = Mathf.Min(distanceToMove, maxDistanceFromPlayer - currentDistance);
            hold_pos.position -= directionToPlayer * distanceToMoveClamped;
        }
    }

    public void SetHoldPosition(float newDistance)
    {
        // Clamp the new distance between minDistanceFromPlayer and maxDistanceFromPlayer
        float clampedDistance = Mathf.Clamp(newDistance, minDistanceFromPlayer, maxDistanceFromPlayer);
        
        // Calculate the direction from the camera to the hold position
        Vector3 directionToPlayer = (hold_pos.position - Camera.main.transform.position).normalized;

        // Set the hold position to the new distance from the camera
        hold_pos.position = Camera.main.transform.position + directionToPlayer * clampedDistance;
    }
}

