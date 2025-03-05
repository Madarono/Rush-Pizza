using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum LicenseState
{
    Empty,
    Available,
    Bought,
}

[System.Serializable]
public class GuaranteedLicense
{
    public MerchantGoods license;
    public LicenseState licenseState;
}

public class Mission : MonoBehaviour, IDataPersistence
{
    public GuaranteedLicense[] licenses;
    public Merchant merchantScript;
    public bool canSave = false;
    private List<int> saveState = new List<int>();
    private int[] intSaveState;


    public void SaveData(GameData data)
    {
        if(canSave)
        {
            ChangeStateIntoInt();
            data.saveState = this.saveState.ToArray();
        }
    }

    public void LoadData(GameData data)
    {
        this.intSaveState = data.saveState;
        ChangeIntIntoState();
    }

    public void Refresh()
    {
        if(merchantScript == null)
        {
            return;
        }
        merchantScript.guarenteedGoods.Clear();

        List<MerchantGoods> availableLicenses = new List<MerchantGoods>();
        
        for(int i = 0; i < licenses.Length; i++)
        {
            if(licenses[i].licenseState == LicenseState.Available)
            {
                availableLicenses.Add(licenses[i].license);
            }
        }

        merchantScript.guarenteedGoods = availableLicenses;
    }

    public void BoughtItem(MerchantGoods goods)
    {
        for(int i = 0; i < licenses.Length; i++)
        {
            if(licenses[i].license == goods)
            {
                licenses[i].licenseState = LicenseState.Bought;
                break;
            }
        }

        Refresh();
    }

    void ChangeStateIntoInt()
    {
        foreach(GuaranteedLicense guaranteed in licenses)
        {
            switch(guaranteed.licenseState)
            {
                case LicenseState.Empty:
                    saveState.Add(0);
                    break;

                case LicenseState.Available:
                    saveState.Add(1);
                    break;

                case LicenseState.Bought:
                    saveState.Add(2);
                    break;

                default:
                    Debug.Log("No compatible type");
                    break;
            }
        }
    }

    void ChangeIntIntoState()
    {
        if(intSaveState.Length == 0)
        {
            return;
        }

        for(int i = 0; i < licenses.Length; i++)
        {
            switch(intSaveState[i])
            {
                case 0:
                    licenses[i].licenseState = LicenseState.Empty;
                    break;

                case 1:
                    licenses[i].licenseState = LicenseState.Available;
                    break;

                case 2:
                    licenses[i].licenseState = LicenseState.Bought;
                    break;

                default:
                    Debug.Log("No compatible integer");
                    break;
            }
        }
    }
}
