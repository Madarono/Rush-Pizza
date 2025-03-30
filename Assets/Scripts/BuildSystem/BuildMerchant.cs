using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class DecorRequirement
{
    public MerchantGoods good;
    public bool gotSold;
}

public class BuildMerchant : MonoBehaviour, IDataPersistence
{
    public DecorRequirement[] decorReq;
    public int maximumToMerchant = 2;
    public Merchant merchant;
    
    [HideInInspector]public bool[] a_soldSafe;
    private List<bool> boolSafe = new List<bool>();

    public bool canSave;

    public void SaveData(GameData data)
    {
        if(canSave)
        {
            SaveBools();
            data.decorSafe = this.a_soldSafe;
        }
    }

    public void LoadData(GameData data)
    {
        this.a_soldSafe = data.decorSafe;
        LoadBools();
    }

    void SaveBools()
    {
        boolSafe.Clear();

        foreach(DecorRequirement req in decorReq)
        {
            boolSafe.Add(req.gotSold);
        }

        a_soldSafe = boolSafe.ToArray();
    }
    void LoadBools()
    {
        if(a_soldSafe.Length == 0)
        {
            return;
        }
        
        for(int i = 0; i < decorReq.Length; i++)
        {
            decorReq[i].gotSold = a_soldSafe[i];
        }
    }

    public void SendToMerchant()
    {
        if(merchant == null)
        {
            return;
        }

        List<MerchantGoods> l_goods = new List<MerchantGoods>();
        l_goods = merchant.goods.ToList();

        List<DecorRequirement> req = new List<DecorRequirement>();
        req = decorReq.ToList();
        for(int i = req.Count - 1; i >= 0; i--)
        {
            if(req[i].gotSold)
            {
                req.RemoveAt(i);
            }
        }

        if(req.Count == 0) //If all gotSold
        {
            return;
        }

        for(int i = 0; i < maximumToMerchant; i++)
        {
            int random = Random.Range(0, req.Count);
            
            if(!l_goods.Contains(req[random].good))
            {
                l_goods.Add(req[random].good);
            }
            
            req.RemoveAt(random);
            
            if(req.Count == 0)
            {
                break;
            }
        }

        merchant.goods = l_goods.ToArray();
    }

    public void UpdateSold(MerchantGoods good)
    {
        for(int i = 0; i < decorReq.Length; i++)
        {
            if(good == decorReq[i].good)
            {
                decorReq[i].gotSold = true;
                break;
            }
        }
    } 

}