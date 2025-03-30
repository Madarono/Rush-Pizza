using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Supply,
    License,
    Decoration
}

[CreateAssetMenu(fileName="Item", menuName="Custom/Goodies", order=1)]
public class MerchantGoods : ScriptableObject
{
    [Header("Ingrediant Type")]
    public PizzaOptions ingrediantType;
    public bool neutral = false;
    public bool putOnce = false; 

    [Header("Information")]
    public Sprite icon;
    public float price;

    public string infoEnglish;
    public string infoDeutsch;

    [Header("Type")]
    public ItemType itemType;

    [Header("If license")]
    public int licenseID;

    [Header("If Supply")]
    public int supplyCount;

    [Header("If Decor")]
    public int decorID;
}