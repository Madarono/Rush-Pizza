using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusScale : MonoBehaviour
{
    [Header("Increase Scale When Hovering")]
    Vector3 scale; //Scale
    public float increaseMultiplyer;
    public bool increase;
    public float maxIncrease = 1.2f;
    public float minIncrease = 0.759196f;

    public void IncreaseInScale()
    {
        increase = true;
    }

    public void DecreaseInScale()
    {
        increase = false;
    }

    void Update()
    {
        scale = transform.localScale;
        if(increase && scale.x < maxIncrease)
        {
            scale.x += Time.unscaledDeltaTime * increaseMultiplyer;
            scale.y += Time.unscaledDeltaTime * increaseMultiplyer;
            scale.z += Time.unscaledDeltaTime * increaseMultiplyer;
            transform.localScale = scale;
        }
        else if(!increase && scale.x > minIncrease)
        {
            scale.x -= Time.unscaledDeltaTime * increaseMultiplyer;
            scale.y -= Time.unscaledDeltaTime * increaseMultiplyer;
            scale.z -= Time.unscaledDeltaTime * increaseMultiplyer;
            transform.localScale = scale;
        }

        if(scale.x > maxIncrease)
        {
            scale.x = maxIncrease;
            scale.y = maxIncrease;
            scale.z = maxIncrease;
            transform.localScale = scale;
        }
        else if(scale.x < minIncrease)
        {
            scale.x = minIncrease;
            scale.y = minIncrease;
            scale.z = minIncrease;
            transform.localScale = scale;
        }
    }
}
