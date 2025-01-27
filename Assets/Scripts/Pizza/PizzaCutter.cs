using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectAndMaterials
{
    public MeshRenderer mesh;
    public Material onMaterial;
    public Material offMaterial;
}

public class PizzaCutter : MonoBehaviour
{
    public bool isUsed = false;
    public bool isOnPizza = false;
    public Settings settings;
    public LayerMask pizzaLayer;
    public float distanceAwayAllowed = 20f;
    public ObjectAndMaterials[] materials;
    private Transform cameraTransform;
    private PizzaHolder pizza;

    void Update()
    {
        if(isUsed)
        {
            CheckForPizza();
            float distance = Vector3.Distance(transform.position, cameraTransform.position);
            if(distance > distanceAwayAllowed)
            {
                TurnOff();
            }
        }
    }

    public void TurnOn(Transform camera)
    {
        isUsed = true;
        cameraTransform = camera;
        for(int i = 0; i < materials.Length; i++)
        {
            materials[i].mesh.material = materials[i].onMaterial;
        }
    }

    public void TurnOff()
    {
        isUsed = false;
        if(pizza != null)
        {
            pizza.visualCutter.SetActive(false);
        }
        for(int i = 0; i < materials.Length; i++)
        {
            materials[i].mesh.material = materials[i].offMaterial;
        }
    }

    void CheckForPizza()
    {
        RaycastHit hit;

        if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, settings.lookRange, pizzaLayer))
        {
            pizza = hit.collider.gameObject.GetComponent<PizzaHolder>();
            if(pizza == null || !pizza.pizza.isCooked)
            {
                return;
            }

            pizza.visualCutter.SetActive(true);
            isOnPizza = true;
        }
        else
        {
            if(pizza != null)
            {
                pizza.visualCutter.SetActive(false);
                isOnPizza = false;
            }
        }
    }
}
