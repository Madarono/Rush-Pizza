using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public Light[] lightBulbs;
    public Light[] pointLights;
    public float intensity;
    public float maxIntensity;
    public float minIntensity;
    public float multiplyer = 4f;

    public void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if(scroll != 0)
        {
            intensity += (scroll * multiplyer);
            if(intensity > maxIntensity)
            {
                intensity = maxIntensity;
            }
            else if(intensity < minIntensity)
            {
                intensity = minIntensity;
            }
            UpdateBulbs();
        }
    }

    public void UpdateBulbs()
    {
        for(int i = 0; i < lightBulbs.Length; i++)
        {
            lightBulbs[i].intensity = this.intensity;
            pointLights[i].intensity = this.intensity / 0.6f;
        }
    }
}
