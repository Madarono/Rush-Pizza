using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MerchantDisplay : MonoBehaviour
{
    public MerchantItem merchantItem;
    public Merchant merchant;
    public Image icon;
    public float price;

    [Header("Moving")]
    public Transform placeToGo;
    public float moveSpeed;

    public void Update()
    {
        if(placeToGo != null)
        {
            transform.position = Vector3.Lerp(transform.position, placeToGo.position, Time.unscaledDeltaTime * moveSpeed);
        }
    }

    public void UnCart()
    {
        merchant.UnCart(merchantItem, this);
        Destroy(gameObject);
    }
}