using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BuildVisual : MonoBehaviour
{
    public bool isSelected;
    public GameObject selection;
    public Image icon;
    public RectTransform rect;
    public GameObject decorPrefab;
    public int decorID;

    public Vector3 selectedScale;
    public Vector3 normalScale;
    public float speedOfChange;

    public void Refresh()
    {
        selection.SetActive(isSelected);
    }

    void FixedUpdate()
    {
        if(isSelected)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, selectedScale, speedOfChange * Time.deltaTime);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, normalScale, speedOfChange * Time.deltaTime);
        }
    }
} 