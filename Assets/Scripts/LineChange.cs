using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineChange : MonoBehaviour
{
    public Settings settings;
    public RectTransform rect;
    public float[] width;
    Vector2 englishWidth;
    Vector2 deutschWidth;

    void Start()
    {
        englishWidth = new Vector2(width[0], rect.sizeDelta.y);
        deutschWidth = new Vector2(width[1], rect.sizeDelta.y);
    }

    void Update()
    {
        if(settings.english)
        {
            rect.sizeDelta = englishWidth;
        }
        else
        {
            rect.sizeDelta = deutschWidth;
        }
    }
}
