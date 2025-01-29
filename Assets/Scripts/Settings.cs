using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public class Settings : MonoBehaviour, IDataPersistence
{
    public float money;
    public bool canSaveMoney = true; //Use this when starting the day
    public TextMeshProUGUI moneyCounter;
    public TextMeshProUGUI negativeMoneyCounter;
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
    public TimeChanges timeChanges;

    public void SaveData(GameData data)
    {
        if(canSaveMoney)
        {
            data.money = this.money;
        }
        // Will make a universal script so that it works in mainmenu and outside of it. 
        // data.english = this.english;
        // data.holdCrouch = this.holdCrouch;
        // data.h24Format = this.h24Format;
        // data.crouch = this.crouch;
        // data.throwKey = this.throwKey;
        // data.jump = this.jump;
        // data.sprint = this.sprint;
    }

    public void LoadData(GameData data)
    {
        this.money = data.money;
        this.english = data.english;
        this.holdCrouch = data.holdCrouch;
        this.h24Format = data.h24Format;
        this.crouch = data.crouch;
        this.throwKey = data.throwKey;
        this.jump = data.jump;
        this.sprint = data.sprint;
        RefreshMoneyCounter();
        timeChanges.UpdateTime();
    }

    public void RefreshMoneyCounter()
    {
        if(money < 0)
        {
            float absoluteMoney = -money;
            if(english)
            {
                negativeMoneyCounter.text = "-$" + absoluteMoney.ToString("F2");
            }
            else
            {
                negativeMoneyCounter.text = "-€" + absoluteMoney.ToString("F2", new CultureInfo("de-DE"));
            }

            negativeMoneyCounter.gameObject.SetActive(true);
            moneyCounter.gameObject.SetActive(false);

            if(english)
            {
                moneyVisual.sprite = usd;
            }
            else
            {
                moneyVisual.sprite = euro;
            }
            return;
        }

        if(english)
        {
            moneyCounter.text = "$" + money.ToString("F2");
        }
        else
        {
            moneyCounter.text = "€" + money.ToString("F2", new CultureInfo("de-DE"));
        }
        moneyCounter.gameObject.SetActive(true);
        negativeMoneyCounter.gameObject.SetActive(false);
    }

    public void AddWithoutVisual(float amount)
    {
        money += amount;
        RefreshMoneyCounter();
    }

    public void AddToMoney(float amount)
    {
        money += amount;
        RefreshMoneyCounter();
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
