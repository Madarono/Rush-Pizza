using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeChoosing : MonoBehaviour
{
    public Recipe recipe;
    public RecipeSystem recipeSystem;

    public void SendRecipeToSystem()
    {
        recipeSystem.ShowPizza(recipe);
    }
}
