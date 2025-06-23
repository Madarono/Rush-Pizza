using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SupplyRequirements
{
    public int maxSupply;
    public int minSupply;
    public Material supplyMaterial;
    public int numberOfBars;
}

public class ToppingBox : MonoBehaviour
{
    public ToppingSO topping;
    public bool isUsed = false;
    
    public Animator boxAnimation;
    
    [Header("Supply Visual")]
    public SupplyRequirements[] supplyReq;
    public int supply;
    public int cacheSupply;
    public MeshRenderer[] barMaterial;
    public GameObject[] barAmount;

    [Header("Id for Supply.cs")]
    public int id;

    void Start()
    {
        UpdateSupply();
    }


    public void OpenUseBox()
    {
        if(boxAnimation == null)
        {
            return;
        }

        boxAnimation.SetBool("Open", true);
    }
    public void CloseUseBox()
    {
        if(boxAnimation == null)
        {
            return;
        }

        boxAnimation.SetBool("Open", false);
    }

    public void UpdateSupply() //1: 24, 2: 48, 3: 72, 4: 96, 5:120
    {
        if(supply == 0)
        {
            for(int i = 0; i < barAmount.Length; i++)
            {
                barAmount[i].SetActive(false);
            }
            return;
        }

        for(int i = 0; i < supplyReq.Length; i++)
        {
            if(supply <= supplyReq[i].maxSupply && supply >= supplyReq[i].minSupply)
            {
                for(int o = 0; o < barMaterial.Length; o++)
                {
                    barMaterial[o].material = supplyReq[i].supplyMaterial;
                }
                for(int o = 0; o < barAmount.Length; o++)
                {
                    if(o <= supplyReq[i].numberOfBars)
                    {
                        barAmount[o].SetActive(true);
                    }
                    else
                    {
                        barAmount[o].SetActive(false);
                    }
                }

                // cacheSupply = supplyReq[i].minSupply;
                break;
            }
        }
    }
}
