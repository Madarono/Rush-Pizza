using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Outfit
{
    public GameObject[] clothes;
}

public class Cosmetics : MonoBehaviour
{
    public GameObject[] allClothes;
    
    public GameObject[] shirts;
    public GameObject[] hats;

    // [Header("Outfit")]
    // public int changeForOutfit = 25;
    // public bool wearOutfit;
    // public Outfit[] outfits;

    [Header("Skin Tone")]
    public Material[] skinTone;
    public MeshRenderer[] meshRend;

    void Start()
    {
        ChangeOutfit();
        ChangeSkin();
    }

    public void ChangeOutfit()
    {
        foreach(GameObject obj in allClothes)
        {
            obj.SetActive(false);
        }

        // int chance = Random.Range(0, 100);
        // if(chance >= changeForOutfit || wearOutfit)
        // {
        //     int randomOutfit = Random.Range(0, outfits.Length);
        //     for(int i = 0; i < outfits[randomOutfit].clothes.Length; i++)
        //     {
        //         outfits[randomOutfit].clothes[i].SetActive(true);
        //     }
        //     return;
        // }

        int randomShirt = Random.Range(0, shirts.Length);
        int randomHat = Random.Range(0, hats.Length);

        hats[randomHat].SetActive(true);
        shirts[randomShirt].SetActive(true);
    }

    public void ChangeSkin()
    {
        int randomSkin = Random.Range(0, skinTone.Length);
        foreach(MeshRenderer rend in meshRend)
        {
            rend.material = skinTone[randomSkin];
        }
    }
}
