using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class VisualTopping
{
    public PizzaOptions topping;
    public GameObject visual;
    public GameObject ingrediantVisual;
    public GameObject notifictionVisual;
}

[System.Serializable]
public class VisualRecipe
{
    public bool isVisible;
    public GameObject recipe;
}

[System.Serializable]
public class VisualNotification
{
    public string englishInfo;
    public string deutschInfo;
    public PizzaOptions[] pizzaToppings;
}

public class RecipeSystem : WindowOpening, IDataPersistence
{
    public Controls controls;
    public Settings settings;

    [Header("Showing Pizza")]
    public GameObject pizza;
    public VisualTopping[] visualTopping;
    public GameObject cheeseVisual;
    public GameObject sauceVisual;
    
    public TextMeshProUGUI pizzaName;
    public GameObject cheeseCheckmark;
    public GameObject sauceCheckmark;

    [Header("Recipe Visual")]
    public VisualRecipe[] visualRecipe;

    [Header("Notification Visual")]
    public VisualNotification[] visualNoti;
    public GameObject notification;
    public TextMeshProUGUI notificationVisual;
    public float notificationDuration = 4f;

    public void SaveData(GameData data)
    {
        data.isVisible = new bool[this.visualRecipe.Length];
        for(int i = 0; i < visualRecipe.Length; i++)
        {
            data.isVisible[i] = visualRecipe[i].isVisible;
        }
    }

    public void LoadData(GameData data)
    {
        for(int i = 0; i < visualRecipe.Length; i++)
        {
            visualRecipe[i].isVisible = data.isVisible[i];
        }
        RefreshRecipes();
    }


    public override void Start()
    {
        base.Start();
        HidePizza();
        notification.SetActive(false);
    }
    

    public override void OpenWindow()
    {
        base.OpenWindow();
        controls.CloseWindow();
    }
    
    public override void CloseWindow()
    {
        base.CloseWindow();
        HidePizza();
    }

    public void ShowPizza(Recipe recipe)
    {
        HidePizza();
        pizza.SetActive(true);

        for(int i = 0; i < recipe.toppings.Length; i++)
        {
            for(int o = 0; o < visualTopping.Length; o++)
            {
                if(recipe.toppings[i] == visualTopping[o].topping)
                {
                    visualTopping[o].visual.SetActive(true);
                    visualTopping[o].ingrediantVisual.SetActive(true);
                    break;
                }
            }
        }

        if(recipe.cheese)
        {
            cheeseVisual.SetActive(true);
            cheeseCheckmark.SetActive(true);
        }
        if(recipe.sauce)
        {
            sauceVisual.SetActive(true);
            sauceCheckmark.SetActive(true);
        }

        if(settings.english)
        {
            pizzaName.text = recipe.englishName;
        }
        else
        {
            pizzaName.text = recipe.deutschName;
        }
    }

    void HidePizza()
    {
        foreach(VisualTopping topping in visualTopping)
        {
            topping.visual.SetActive(false);

            if(topping.ingrediantVisual != null)
            {
                topping.ingrediantVisual.SetActive(false);
            }
        }

        cheeseVisual.SetActive(false);
        sauceVisual.SetActive(false);
        cheeseCheckmark.SetActive(false);
        sauceCheckmark.SetActive(false);
        pizza.SetActive(false);
    }

    public void VisibleRecipe(int index)
    {
        if(visualRecipe[index].isVisible)
        {
            RefreshRecipes();
            return;
        }

        StartCoroutine(Notification(index));
        visualRecipe[index].isVisible = true;
        RefreshRecipes();
    }

    IEnumerator Notification(int index)
    {
        foreach(VisualTopping topping in visualTopping)
        {
            topping.notifictionVisual.SetActive(false);
        }

        for(int i = 0; i < visualNoti[index].pizzaToppings.Length; i++)
        {
            for(int o = 0; o < visualTopping.Length; o++)
            {
                if(visualNoti[index].pizzaToppings[i] == visualTopping[o].topping)
                {
                    visualTopping[o].notifictionVisual.SetActive(true);
                    break;
                }
            }
        }

        if(settings.english)
        {
            notificationVisual.text = visualNoti[index].englishInfo;
        }
        else
        {
            notificationVisual.text = visualNoti[index].deutschInfo;
        }

        notification.SetActive(true);
        yield return new WaitForSeconds(notificationDuration);
        notification.SetActive(false);
    }

    void RefreshRecipes()
    {
        foreach(VisualRecipe recipe in visualRecipe)
        {
            if(recipe.isVisible)
            {
                recipe.recipe.SetActive(true);
            }
            else
            {
                recipe.recipe.SetActive(false);
            }
        }
    }
}