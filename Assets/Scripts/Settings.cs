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
    public Mission mission;
    public Image moneyVisual;
    public Sprite usd;
    public Sprite euro;

    [Header("IngrediantsBought")]
    public PizzaOptions[] ingrediantsAvailable; //For Merchant.cs

    [Header("Cash Register Visual")]
    public SoundManager sound;

    public TextMeshPro[] registervisual = new TextMeshPro[2]; 
    public float durationvisible = 1.5f;
    public Animator cashRegisterAnimator;
    public float durationOfAnimation;
    public float delaySound = 1f;

    [Header("Keybinds and Modifications")]
    public bool holdCrouch;
    public KeyCode crouch = KeyCode.C;
    public KeyCode throwKey = KeyCode.R;
    public KeyCode jump = KeyCode.Space;
    public KeyCode sprint = KeyCode.LeftShift;
    public KeyCode pause = KeyCode.Tab;
    public KeyCode showBrief = KeyCode.B;
    public KeyCode zoom = KeyCode.Q;
    public KeyCode dailyChallenge = KeyCode.O;

    public KeyCode buildMode = KeyCode.Y;
    public KeyCode changeMode = KeyCode.H;

    public float throwForce = 10f;
    public float lookRange = 7f;

    public bool oneTimeCut = true;
    public bool english = true;
    public bool h24Format = false;
    public bool enableVoice = true;
    public TimeChanges timeChanges;

    public void SaveData(GameData data)
    {
        if(canSaveMoney)
        {
            data.money = this.money;
        }
    }

    public void LoadData(GameData data)
    {
        this.money = data.money;

        this.holdCrouch = data.holdCrouch;
        this.h24Format = data.h24Format;

        RefreshMoneyCounter();
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
        if(english)
        {
            moneyVisual.sprite = usd;
        }
        else
        {
            moneyVisual.sprite = euro;
        }
        moneyCounter.gameObject.SetActive(true);
        negativeMoneyCounter.gameObject.SetActive(false);
    }

    public void AddWithoutVisual(float amount)
    {
        if(amount > 0)
        {
            mission.moneyGained += amount;
        }
        money += amount;
        RefreshMoneyCounter();
    }

    public void AddToMoney(float amount)
    {
        if(amount > 0)
        {
            mission.moneyGained += amount;
        }
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

    public void RegisterGetSound()
    {
        sound.GenerateSound(cashRegisterAnimator.gameObject.transform.position, sound.registerGet, true, .6f);
    }

    public void RegisterLoseSound()
    {
        sound.GenerateSound(cashRegisterAnimator.gameObject.transform.position, sound.registerLose, true, .6f);
    }

    IEnumerator UnactivateRegisterVisual(bool positive)
    {
        yield return new WaitForSeconds(delaySound);
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
