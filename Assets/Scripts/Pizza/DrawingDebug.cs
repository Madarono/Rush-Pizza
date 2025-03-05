using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingDebug : MonoBehaviour
{
    public Settings settings;
    public SauceDrawing sauceDrawing;
    private ToppingSO topping;

    [Header("Project Model")]
    public GameObject projectModel;
    public Material projectMaterial; 
    
    [Header("Runtime")]
    public GameObject activeProjectModel;
    public float projectModelYOffset;

    public void Update()
    {
        topping = sauceDrawing.topping;
        CheckPizza();
    }

    void CheckPizza()
    {
        if(topping == null)
        {
            if(activeProjectModel != null)
            {
                Destroy(activeProjectModel);
                activeProjectModel = null;
            }
            return;
        }

        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, settings.lookRange, topping.drawingSurfaceLayer))
        {
            PizzaHolder pizza = hit.collider.gameObject.GetComponent<PizzaHolder>();

            if(pizza == null)
            {
                return;
            }

            if(pizza.pizza.isCooked || pizza == null)
            {
                if(activeProjectModel != null)
                {
                    Destroy(activeProjectModel);
                    activeProjectModel = null;
                }
                return;
            }

            projectModel = topping.toppingPrefab[0];
           
            if(activeProjectModel == null)
            {
                activeProjectModel = Instantiate(projectModel, hit.point, Quaternion.identity);
                activeProjectModel.transform.up = hit.normal;
                activeProjectModel.transform.Rotate(
                    topping.rotationOffsets[0], 
                    topping.rotationOffsets[1], 
                    topping.rotationOffsets[2], 
                    Space.Self
                );
                UpdateProjectModel();
            }

            activeProjectModel.transform.position = new Vector3(hit.point.x, hit.point.y + projectModelYOffset, hit.point.z);
        }
        else
        {
            if(activeProjectModel != null)
            {
                Destroy(activeProjectModel);
                activeProjectModel = null;
            }
        }
    }

    void UpdateProjectModel()
    {
        if(topping == null)
        {
            return;
        }
        
        MeshRenderer projectRenderer = activeProjectModel.GetComponent<MeshRenderer>();
        projectRenderer.material = projectMaterial;
    }

}
