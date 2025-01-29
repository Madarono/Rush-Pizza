using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LanguageStrings : MonoBehaviour
{
    public Settings settings;
    public TextMeshProUGUI text;
    public string english;
    public string deutsch;

    void Awake()
    {
        if(settings.english)
        {
            text.text = english;
        }
        else
        {
            text.text = deutsch;
        }
    }

    void Update()
    {
        if(settings.english)
        {
            text.text = english;
        }
        else
        {
            text.text = deutsch;
        }
    }
}
