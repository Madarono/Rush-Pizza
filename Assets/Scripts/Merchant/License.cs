using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class L_Box
{
    public PizzaOptions ingrediant;
    public GameObject box;
    public GameObject highlight;
    public GameObject toppingBox;
    public bool hasPlaced = false;
}

[System.Serializable]
public class LicenseDialog
{
    public PizzaOptions[] ingrediantsNeeded;
    public PizzaOptions[] stopIngrediants;
    public Dialog dialog;
}

public class License : MonoBehaviour, IDataPersistence
{
    public L_Box[] licenseBoxes;
    private bool[] boxesPlaced;
    public bool canSave;
    public Transform spawnPoint;
    public Quaternion boxRotation;

    [Header("Applying License")]
    public Settings settings;
    public CustomerManager manager;
    public LicenseDialog[] allDialogs;

    public void SaveData(GameData data)
    {
        if(canSave)
        {
            SetBoolData();
            data.boxesPlaced = this.boxesPlaced;
        }
    }

    public void LoadData(GameData data)
    {
        this.boxesPlaced = data.boxesPlaced;
        LoadBoolData();
        Refresh();
    }

    public void InstantiateBox(int id)
    {
        GameObject go = Instantiate(licenseBoxes[id].box, spawnPoint.position, Quaternion.identity);
        go.transform.rotation = Quaternion.Euler(boxRotation.x, boxRotation.y, boxRotation.z);
        LicenseBox boxScript = go.GetComponent<LicenseBox>();
        boxScript.highlight = licenseBoxes[id].highlight;
        boxScript.toppingBox = licenseBoxes[id].toppingBox;
        boxScript.license = this;
        boxScript.licenseId = id;
    }

    public void UpdateValues(int id)
    {
        licenseBoxes[id].hasPlaced = true;
        Refresh();
    }

    void SetBoolData()
    {
        boxesPlaced = new bool[licenseBoxes.Length];
        for(int i = 0; i < licenseBoxes.Length; i++)
        {
            boxesPlaced[i] = licenseBoxes[i].hasPlaced;
        }
    }

    void LoadBoolData()
    {
        for(int i = 0; i < licenseBoxes.Length; i++)
        {
            licenseBoxes[i].hasPlaced = boxesPlaced[i];
        }
    }

    void Refresh()
    {
        List<PizzaOptions> ingrediants = new List<PizzaOptions>();
        ingrediants.Add(PizzaOptions.Cheese);
        ingrediants.Add(PizzaOptions.Sauce);

        for(int i = 0; i < licenseBoxes.Length; i++)
        {
            licenseBoxes[i].highlight.SetActive(false);
            
            if(licenseBoxes[i].hasPlaced)
            {
                licenseBoxes[i].toppingBox.SetActive(true);
                ingrediants.Add(licenseBoxes[i].ingrediant);
            }
            else
            {
                licenseBoxes[i].toppingBox.SetActive(false);
            }
        }

        settings.ingrediantsAvailable = ingrediants.ToArray();

        List<Dialog> dialogsAllowed = new List<Dialog>();

        foreach(LicenseDialog dialog in allDialogs)
        {
            bool[] pass = new bool[dialog.ingrediantsNeeded.Length];
            for(int i = 0; i < dialog.ingrediantsNeeded.Length; i++)
            {
                for(int o = 0; o < settings.ingrediantsAvailable.Length; o++)
                {
                    if(dialog.ingrediantsNeeded[i] == settings.ingrediantsAvailable[o])
                    {
                        pass[i] = true;
                        break;
                    }
                }
            }

            for(int i = 0; i < dialog.stopIngrediants.Length; i++)
            {
                for(int o = 0; o < settings.ingrediantsAvailable.Length; o++)
                {
                    if(dialog.stopIngrediants[i] == settings.ingrediantsAvailable[o])
                    {
                        for(int p = 0; p < pass.Length; p++)
                        {
                            pass[p] = false;
                        }
                        break;
                    }

                }
            }

            bool canPut = true;
            for(int i = 0; i < pass.Length; i++)
            {
                if(pass[i])
                {
                    continue;
                }

                canPut = false;
                break;
            }

            if(canPut)
            {
                dialogsAllowed.Add(dialog.dialog);
            }
        }

        manager.orders = dialogsAllowed.ToArray();
    }
}