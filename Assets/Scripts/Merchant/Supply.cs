using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IngrediantSupply
{
    public PizzaOptions topping;
    public ToppingBox box;
    public int freeSupply;
    public int maxSupply;
}

public class Supply : MonoBehaviour, IDataPersistence
{
    public IngrediantSupply[] ingrediantSupply;
    private int[] sumSupply;
    public bool canSave;

    public void SaveData(GameData data)
    {
        if(canSave)
        {
            ConvertToArray();
            data.sumSupply = this.sumSupply;
        }

    }

    public void LoadData(GameData data)
    {
        this.sumSupply = data.sumSupply;
        ConvertToIngrediantSupply();
    }

    void ConvertToArray()
    {
        List<int> sum = new List<int>();
        for(int i = 0; i < ingrediantSupply.Length; i++)
        {
            sum.Add(ingrediantSupply[i].freeSupply);
        }

        sumSupply = sum.ToArray();
    }

    void ConvertToIngrediantSupply()
    {
        for(int i = 0; i < ingrediantSupply.Length; i++)
        {
            ingrediantSupply[i].freeSupply = sumSupply[i];
        }
    }

    public void PutToSupply(PizzaOptions topping, int amount)
    {
        for(int i = 0; i < ingrediantSupply.Length; i++)
        {
            if(ingrediantSupply[i].topping == topping)
            {
                ingrediantSupply[i].freeSupply += amount;
                if(ingrediantSupply[i].freeSupply > ingrediantSupply[i].maxSupply)
                {
                    ingrediantSupply[i].freeSupply = ingrediantSupply[i].maxSupply;
                }
                break;
            }
        }
    }
}
