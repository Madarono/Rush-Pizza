using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepScale : MonoBehaviour
{
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }
    void Update()
    {
        transform.localScale = originalScale;
    }
}
