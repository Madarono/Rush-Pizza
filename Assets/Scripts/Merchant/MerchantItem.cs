using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MerchantItem : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI headerType;
    public TextMeshProUGUI info;
    public Merchant merchant;
    public MerchantGoods goods;

    public void AddToCart()
    {
        merchant.AddToCart(this);
    }
}