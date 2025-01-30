using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MenuTab
{
    public GameObject window;

    public Image[] buttonGraphics;
    public Color[] correspondingClosed;
    public Color[] correspondingOpen;
}

public class Tabs : MonoBehaviour
{
    public MenuTab[] menus;

    void Start()
    {
        ResetAllTabs();
    }

    public void ResetAllTabs()
    {
        for(int i = 0; i < menus.Length; i++)
        {
            for(int q = 0; q < menus[i].buttonGraphics.Length; q++)
            {
                menus[i].buttonGraphics[q].color = menus[i].correspondingClosed[q]; 
            }
            menus[i].window.SetActive(false);
        }
    }

    public void ChangeTabs(int index)
    {
        ResetAllTabs();

        menus[index].window.SetActive(true);
        for(int q = 0; q < menus[index].buttonGraphics.Length; q++)
        {
            menus[index].buttonGraphics[q].color = menus[index].correspondingOpen[q]; 
        }
    }
}
