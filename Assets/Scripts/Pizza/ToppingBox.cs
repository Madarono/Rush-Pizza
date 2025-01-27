using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToppingBox : MonoBehaviour
{
    public ToppingSO topping;
    public bool isUsed = false;

    public Animator boxAnimation;

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
