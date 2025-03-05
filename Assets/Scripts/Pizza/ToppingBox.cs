using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToppingBox : MonoBehaviour
{
    public ToppingSO topping;
    public bool isUsed = false;
    
    public Animator boxAnimation;
    
    [Header("Id for Supply.cs")]
    public int id;


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
}
