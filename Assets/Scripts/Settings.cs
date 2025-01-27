using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public class Settings : MonoBehaviour
{
    public float money;
    public TextMeshProUGUI moneyCounter;
    public Image moneyVisual;
    public Sprite usd;
    public Sprite euro;

    [Header("Cash Register Visual")]
    public TextMeshPro[] registervisual = new TextMeshPro[2]; 
    public float durationvisible = 1.5f;
    public Animator cashRegisterAnimator;
    public float durationOfAnimation;

    [Header("Keybinds and Modifications")]
    public bool holdCrouch;
    public KeyCode crouch = KeyCode.C;
    public KeyCode throwKey = KeyCode.R;
    public KeyCode jump = KeyCode.Space;
    public KeyCode sprint = KeyCode.LeftShift;
    public float throwForce = 10f;
    public float lookRange = 7f;

    public bool oneTimeCut = true;
    public bool english = true;
    public bool h24Format = false;


    void Start()
    {
        if(english)
        {
            moneyCounter.text = "$" + money.ToString("F2");
        }
        else
        {
            moneyCounter.text = "€" + money.ToString("F2", new CultureInfo("de-DE"));
        }
        if(english)
        {
            moneyVisual.sprite = usd;
        }
        else
        {
            moneyVisual.sprite = euro;
        }
    }

    public void AddWithoutVisual(float amount)
    {
        money += amount;
        if(english)
        {
            moneyCounter.text = "$" + money.ToString("F2");
        }
        else
        {
            moneyCounter.text = "€" + money.ToString("F2", new CultureInfo("de-DE"));
        }
    }

    public void AddToMoney(float amount)
    {
        money += amount;
        if(english)
        {
            moneyCounter.text = "$" + money.ToString("F2");
        }
        else
        {
            moneyCounter.text = "€" + money.ToString("F2", new CultureInfo("de-DE"));
        }
        StopAllCoroutines(); //Remove if this causes bugs later on
        StartCoroutine(ShowCashAniamtion());

        //Register Visual
        if(amount > 0)
        {
            StartCoroutine(UnactivateRegisterVisual(true));
            if(english)
            {
                registervisual[0].text = "+$" + amount.ToString("F2");
            }
            else
            {
                registervisual[0].text = "+€" + amount.ToString("F2", new CultureInfo("de-DE"));
            }
        }
        else
        {
            StartCoroutine(UnactivateRegisterVisual(false));
            registervisual[1].gameObject.SetActive(true);
            float removeNegative = -amount;
            if(english)
            {
                registervisual[1].text = "-$" + removeNegative.ToString("F2");
            }
            else
            {
                registervisual[1].text = "-€" + removeNegative.ToString("F2", new CultureInfo("de-DE"));
            }
        }

    } 

    IEnumerator UnactivateRegisterVisual(bool positive)
    {
        yield return new WaitForSeconds(durationOfAnimation);
        if(positive)
        {
            registervisual[0].gameObject.SetActive(true);
        }
        else
        {
            registervisual[1].gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(durationvisible);
        registervisual[0].gameObject.SetActive(false);
        registervisual[1].gameObject.SetActive(false);
    }

    IEnumerator ShowCashAniamtion()
    {
        cashRegisterAnimator.SetBool("ShowCash", true);
        yield return new WaitForSeconds(durationOfAnimation);
        cashRegisterAnimator.SetBool("ShowCash", false);
    }
}
