using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dimentions
{
    public int minReq; // Minimum character count
    public int maxReq; // Maximum character count
    public float yAxis; // Exact Y-axis position
    public float hAxis; // Height of the dialog box
}

public class DialogBox : MonoBehaviour
{
    private RectTransform rect;
    public Dimentions[] dimentions;
    public string dialogString;

    void Start()
    {
        rect = GetComponent<RectTransform>();

        if (rect == null)
        {
            Debug.LogError("RectTransform component is missing!");
        }
        gameObject.SetActive(false);
    }

    public void CheckDimentions(string dialog)
    {
        dialogString = dialog;
        Debug.Log(dialogString.Length);
        int dialogCount = dialog.Length;

        foreach (var dim in dimentions)
        {
            if (dialogCount >= dim.minReq && dialogCount <= dim.maxReq)
            {
                transform.localPosition = new Vector3( transform.localPosition.x, dim.yAxis, transform.localPosition.z);
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, dim.hAxis);

                break;
            }
        }
    }
}
