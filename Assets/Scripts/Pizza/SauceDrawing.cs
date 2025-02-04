using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SauceDrawing : MonoBehaviour
{
    public Settings settings;
    public Stats stats;
    public DragAndDrop dragAndDrop;
    public ToppingSO topping;
    
    private List<Vector3> drawnPath = new List<Vector3>();
    private bool isDrawing;
    private Transform camera;
    private Vector3 lastSpawnPosition;  // Tracks the last spawn position to check distance

    private PizzaHolder pizza;

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
            pizza = hit.collider.gameObject.GetComponent<PizzaHolder>();
            if(pizza.pizza.isCooked)
            {
                return;
            }

            Vector3 hitPoint = hit.point;
    
            // Only spawn sauce if far enough from last position and no other sauce nearby
            if (Vector3.Distance(hitPoint, lastSpawnPosition) > topping.minDistance && 
                ((topping.checkToppinglayer && !SauceNearby(hitPoint)) || !topping.checkToppinglayer))
            {
                drawnPath.Add(hitPoint);

                int randomPrefab = Random.Range(0, topping.toppingPrefab.Length);
                GameObject sauce = Instantiate(topping.toppingPrefab[randomPrefab], hitPoint + Vector3.up * topping.spawnOffset, Quaternion.identity);
                settings.AddWithoutVisual(-topping.priceToPlace);
                stats.toppingStats[topping.indexForStat].moneySpent += topping.priceToPlace;
                
                if (topping.checkForTopping)
                {
                    Vector3[] directions = {
                        sauce.transform.forward,
                        -sauce.transform.forward,
                        sauce.transform.right,
                        -sauce.transform.right
                    };

                    foreach (Vector3 direction in directions)
                    {
                        RaycastHit checkHit;
                        if (Physics.Raycast(sauce.transform.position, direction, out checkHit, 0.1f))
                        {
                            if (checkHit.collider.gameObject.CompareTag(topping.checkToppingTag))
                            {
                                sauce.transform.position = new Vector3(sauce.transform.position.x, sauce.transform.position.y + topping.checkOffset, sauce.transform.position.z);
                                break;
                            }
                        }
                    }
                }

                sauce.transform.up = hit.normal; // Align the sauce with the pizza surface normal
                sauce.transform.Rotate(
                    topping.rotationOffsets[0] + (topping.rotationRandom[0] ? Random.Range(-360f, 360f) : 0f),
                    topping.rotationOffsets[1] + (topping.rotationRandom[1] ? Random.Range(-360f, 360f) : 0f),
                    topping.rotationOffsets[2] + (topping.rotationRandom[2] ? Random.Range(-360f, 360f) : 0f),
                    Space.Self
                );
    
                sauce.transform.SetParent(hit.transform);
                Pizza pizzaScript = sauce.GetComponentInParent<Pizza>();
                pizzaScript.CheckName(topping.name, sauce);
                lastSpawnPosition = hitPoint;  // Update the last spawn position
            }
        }
    }


    bool SauceNearby(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, topping.spawnRadius, topping.toppingLayer);
        return colliders.Length > 0;
    }
}
