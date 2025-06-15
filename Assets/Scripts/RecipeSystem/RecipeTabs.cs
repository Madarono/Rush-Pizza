using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tab
{
    public RectTransform tab;
    public GameObject window;
    public bool isActive;
    public RectTransform[] Xpos;
    public float[] width = new float[2];
}

[System.Serializable]
public class ShowIngrediant
{
    public PizzaOptions topping;
    public GameObject[] obj;
}

public class RecipeTabs : MonoBehaviour
{
    [Header("Tabs")]
    public Tab[] tabs;
    public float speed;

    [Header("Toppings")]
    public Settings settings;
    public ShowIngrediant[] toppings;

    void Start()
    {
        OpenWindow(0);
        UpdateIngrediants();
    }

    public void OpenWindow(int id)
    {
        UpdateIngrediants();
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].window.SetActive(false);
            tabs[i].isActive = false;
        }

        tabs[id].window.SetActive(true);
        tabs[id].isActive = true;
    }

    public void UpdateIngrediants()
    {
        foreach(ShowIngrediant ingrediant in toppings)
        {
            for(int i = 0; i < settings.ingrediantsAvailable.Length; i++)
            {
                if(ingrediant.topping == settings.ingrediantsAvailable[i])
                {
                    for(int o = 0; o < ingrediant.obj.Length; o++)
                    {
                        ingrediant.obj[o].SetActive(true);
                    }
                    break;
                }
                else
                {
                    for(int o = 0; o < ingrediant.obj.Length; o++)
                    {
                        ingrediant.obj[o].SetActive(false);
                    }
                }
            }
        }
    }

    void Update()
    {
        foreach (Tab tab in tabs)
        {
            int i = tab.isActive ? 1 : 0;

            Vector2 targetSize = new Vector2(tab.width[i], tab.tab.sizeDelta.y);

            tab.tab.position = Vector3.Lerp(tab.tab.position, tab.Xpos[i].position, Time.unscaledDeltaTime * speed);
            tab.tab.sizeDelta = Vector2.Lerp(tab.tab.sizeDelta, targetSize, Time.unscaledDeltaTime * speed);
        }
    }

}
