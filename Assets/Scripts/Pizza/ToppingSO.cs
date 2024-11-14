using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Topping", menuName = "Custom/Topping")]
public class ToppingSO : ScriptableObject
{
    public string name;
    public float price;

    [Header("For Drawing")]
    public GameObject toppingPrefab;
    public float[] rotationOffsets = new float[3];
    public bool[] rotationRandom = new bool[3];
    public LayerMask drawingSurfaceLayer; // The surface of the pizza
    public LayerMask toppingLayer; // Checks prefab layer
    public bool checkToppinglayer = false; // This makes sure if it checks the layer or not
    public bool hold = false; // Holding = Draw when holding, else; Draw single
    public float spawnOffset = 0.01f; // Offset in the Z axis when being placed; 
    public float spawnRadius; // Radius to check for existing sauce objects
    public float minDistance; // Minimum distance before spawning another sauce object
}
