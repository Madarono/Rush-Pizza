using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SauceDrawing : MonoBehaviour
{
    public SoundManager sound;
    public Settings settings;
    public Stats stats;
    public DragAndDrop dragAndDrop;
    public ToppingSO topping;
    public Tutorial tutorial;

    [Header("Supply.cs")]
    public int idForSupply;
    public Supply supply;

    private Vector3 hitNormal;
    public Vector3 previousPizzaPos;

    
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
            isDrawing = false;
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

            Vector3 pizzaPos = hit.transform.position;
            
            if(pizzaPos != previousPizzaPos)
            {
                hitNormal = hit.normal;
            }
            previousPizzaPos = hit.transform.position;
            
            if(pizza == null)
            {
                return;
            }
            
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
                sound.GenerateSound(hitPoint, sound.splat, true, 0.4f);

                if(supply.ingrediantSupply[idForSupply].freeSupply > 0)
                {
                    supply.ingrediantSupply[idForSupply].freeSupply--;
                    supply.ingrediantSupply[idForSupply].box.supply = supply.ingrediantSupply[idForSupply].freeSupply;
                    supply.ingrediantSupply[idForSupply].box.UpdateSupply();
                }
                else
                {
                    settings.AddWithoutVisual(-topping.priceToPlace);
                    stats.toppingStats[topping.indexForStat].moneySpent += topping.priceToPlace;
                }
                
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
                if(hitNormal != Vector3.zero)
                {
                    sauce.transform.up = hitNormal;
                }
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

                if(tutorial.states == TutorialStates.PutToppings)
                {
                    tutorial.hasPutToppings = true;
                    tutorial.CheckRequirements();
                }
            }
        }
    }


    bool SauceNearby(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, topping.spawnRadius, topping.toppingLayer);
        return colliders.Length > 0;
    }
}
